// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com -- has extended this class to partially support parenting
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 08-09-14

using Seven;
using Seven.Structures;
using SevenEngine.Imaging;
using Seven.Mathematics;
using SevenEngine.Shaders;
using OpenTK;


namespace SevenEngine.StaticModels
{
  /// <summary>Represents a collection of static meshes that all use the same model-view matrix.</summary>
  public class StaticModel
  {
    protected string _id;
    protected bool _hasChildren;
    protected bool _isChild;
    protected StaticModel _parentModel;
    protected Vector3 _position;
    protected Vector3 _scale;
    protected Quaternion _orientation;
    protected ShaderProgram _shaderOverride;
    protected AvlTree<StaticMesh> _meshes;
    protected AvlTree<StaticModel> _childrenModels;

    /// <summary>Gets the list of meshes that make up this model.</summary>
    public AvlTree<StaticMesh> Meshes { get { return _meshes; } set { _meshes = value; } }
    /// <summary>Gets the list of models parented to this model.</summary>
    public AvlTree<StaticModel> ChildrenModels { get { return _childrenModels; } set { _childrenModels = value; if (value != null) _hasChildren = true; else _hasChildren = false; } }
    /// <summary>Gets the list of models parented to this model.</summary>
    public StaticModel ParentModel { get { return _parentModel; } set { _parentModel = value; if (value != null) _isChild = true; else _isChild = false; } }
    /// <summary>Look-up id for pulling the static model out of the databases.</summary>
    public string Id { get { return _id; } set { _id = value; } }
    /// <summary>Indicates if there are models parented to this model.</summary>
    public bool HasChildren { get { return _hasChildren; }  }
    /// <summary>Indicates if this model is a child of another model.</summary>
    public bool IsChild { get { return _isChild; } }
    /*
    /// <summary>The position vector of this static model (used in rendering transformations).</summary>
    public Vector3 PositionAbsolute
    {   
        get 
        {
            if (_isChild)
                return ParentModel.PositionAbsolute + Quaternion_Multiply_Vector(ParentModel.OrientationAbsolute, _position);
            else
                return _position;
        }
        set { _position = value; }
    }
    /// <summary>The scale vector (scale of each axis separately) of this static model (used in rendering transformations).</summary>
    public Vector3 ScaleAbsolute
    {   
        get 
        {
            if (_isChild)
                return new Vector3(_scale.X*ParentModel.ScaleAbsolute.X , _scale.Y*ParentModel.ScaleAbsolute.Y, _scale.Z*ParentModel.ScaleAbsolute.Z);
            else
                return _scale;
        }
        set { _scale = value; }
    }
    /// <summary>Represents the orientation of a static model by a quaternion rotation.</summary>
    public Quaternion OrientationAbsolute
    {
        get
        {
            Quaternion result;
            if (_isChild)
            {
                result = _orientation * ParentModel.OrientationAbsolute;
                return result;
            }
            else
                return _orientation;
        }
        set { _orientation = value; }
    }
    */
    /// <summary>Represents the position of a static model by a vector in WORLD coordinates (absolute).
    /// Used when rendering the object. Parent models transformations are automatically applied.</summary>
    public Vector3 Position
    {   
        get
        {
            if (_isChild)
            {
                
                return ParentModel.Position + Geometric.Quaternion_Rotate(ParentModel.Orientation, _position); 
            }
            else
                return _position;
        }
        set 
        {
            if (_isChild)
            {
                _position = Geometric.Quaternion_Rotate(ParentModel.Orientation.Inverted(), value) - ParentModel.Position;
            }
            else
                _position = value;
        }
    }
    /// <summary>The scale vector (scale of each axis separately) of this static model (used in rendering transformations).</summary>
    public Vector3 Scale
    {
        get
        {
            return _scale;
        }
        set 
        {
            if (_hasChildren)
            {
                foreach (StaticModel childModel in _childrenModels)
                {
                    childModel.PositionRelative = new Vector3(childModel.PositionRelative.X * value.X, childModel.PositionRelative.Y * value.Y,childModel.PositionRelative.Z * value.Z);
                }
            }
            _scale = value;
        }
    }
    /// <summary>Represents the orientation of a static model by a quaternion rotation in WORLD reference system (absolute).
    /// Used when rendering the object. Parent models transformations are automatically applied.
    /// </summary>
    public Quaternion Orientation
    {
        get
        {
            Quaternion result;
            if (_isChild)
            {
                result = ParentModel.Orientation * _orientation;
                return result;
            }
            else
                return _orientation;
        }
        set 
        {
            if (_isChild)
            {
                _orientation = ParentModel.Orientation.Inverted() * value;
            }
            else
                _orientation = value;
        }
    }
    /// <summary>Represents the position of a static model by a vector in PARENT coordinates (relative).
    /// If this model has no parent, relative position is equal to absolute position.
    /// </summary>
    public Vector3 PositionRelative { get { return _position; } set { _position = value; } }
    /// <summary>The scale vector (scale of each axis separately) of this static model (used in rendering transformations).</summary>
    //public Vector3 ScaleRelative { get { return _scale; } set { _scale = value; } }
    /// <summary>Represents the orientation of a static model by a quaternion rotation in PARENT reference system (relative).
    /// If this model has no parent, relative position is equal to absolute position.
    /// </summary>
    public Quaternion OrientationRelative { get { return _orientation; } set { _orientation = value; } }
    /// <summary>Overrides the default shader while rendering this specific model.</summary>
    public ShaderProgram ShaderOverride { get { return _shaderOverride; } set { _shaderOverride = value; } }

    /// <summary>Creates a blank template for a static model (you will have to construct the model yourself).</summary>
    public StaticModel(string id)
    {
      _id = id;
      _hasChildren = false;
      _isChild = false;
      _shaderOverride = null;
      _meshes = new AvlTree_Linked<StaticMesh>(StaticMesh.CompareTo);
      _childrenModels = new AvlTree_Linked<StaticModel>(StaticModel.CompareTo);
      _parentModel = null;
      _position = new Vector3(0, 0, 0);
      _scale = new Vector3(1, 1, 1);
      _orientation = Quaternion.Identity;
    }

    /// <summary>Creates a static model from the ids provided.</summary>
    /// <param name="staticModelId">The id to represent this model as.</param>
    /// <param name="textures">An array of the texture ids for each sub-mesh of this model.</param>
    /// <param name="meshes">An array of each mesh id for this model.</param>
    /// <param name="meshNames">An array of mesh names for this specific instanc3e of a static model.</param>
    internal StaticModel(string staticModelId, string[] meshNames, string[] meshes, string[] textures)
    {
      if (textures.Length != meshes.Length && textures.Length != meshNames.Length)
        throw new System.Exception("Attempting to create a static model with non-matching number of components.");
      _id = staticModelId;
      _hasChildren = false;
      _isChild = false;
      _meshes = new AvlTree_Linked<StaticMesh>(StaticMesh.CompareTo);
      _childrenModels = new AvlTree_Linked<StaticModel>(StaticModel.CompareTo);
      _parentModel = null;
      for (int i = 0; i < textures.Length; i++)
      {
        StaticMesh mesh = StaticModelManager.GetMesh(meshes[i]);
        mesh.Texture = TextureManager.Get(textures[i]);
        _meshes.Add(mesh);

      }
      _shaderOverride = null;
      _position = new Vector3(0, 0, 0);
      _scale = new Vector3(1, 1, 1);
      _orientation = Quaternion.Identity;
    }

    /// <summary>Creates a static model out of the parameters.</summary>
    /// <param name="staticModelId">The id of this model for look up purposes.</param>
    /// <param name="meshes">A list of mesh ids, textures, and buffer references that make up this model.</param>
    internal StaticModel(string staticModelId, AvlTree<StaticMesh> meshes)
    {
      _id = staticModelId;
      _hasChildren = false;
      _isChild = false;
      _shaderOverride = null;
      _meshes = meshes;
      _childrenModels = new AvlTree_Linked<StaticModel>(StaticModel.CompareTo);
      _parentModel = null;
      _position = new Vector3(0, 0, 0);
      _scale = new Vector3(1, 1, 1);
      _orientation = Quaternion.Identity;
    }

    public static Comparison CompareTo(StaticModel left, StaticModel right)
    {
      int comparison = left.Id.CompareTo(right.Id);
      if (comparison > 0)
        return Comparison.Greater;
      else if (comparison < 0)
        return Comparison.Less;
      else
        return Comparison.Equal;
    }

    public static Comparison CompareTo(StaticModel left, string right)
    {
      int comparison = left.Id.CompareTo(right);
      if (comparison > 0)
        return Comparison.Greater;
      else if (comparison < 0)
        return Comparison.Less;
      else
        return Comparison.Equal;
    }

    public void addChildren(StaticModel child)
    {
        if (child != null)
        {
            //is it already there? if not... (how to check?)
            try
            {
                _childrenModels.Add(child);
            }
            catch(System.Exception e)
            {
                Output.WriteLine(e.Message);
            }
            _hasChildren = true;
            if (child.ParentModel == null || (child.ParentModel != null && StaticModel.CompareTo(child.ParentModel, this) != Comparison.Equal) )
                child.setParent(this);
        }
    }

    public void addChildren(string childName)
    {
        if (childName != null)
        {
            addChildren(StaticModelManager.GetModel(childName));
        }
    }

    public void clearChildren()
    {
        _childrenModels.Clear();
        _hasChildren = false;
    }

    public void setParent(StaticModel parent)
    {
        if (parent != null)
        {
            _parentModel = parent;
            _isChild = true;
            parent.addChildren(this);
        }
    }

    public void setParent(string parentName)
    {
        if (parentName != null)
        {
            setParent(StaticModelManager.GetModel(parentName));
        }
    }

    public void clearParent()
    {
        _parentModel = null;
        _isChild = false;
    }

    public void FreeLookAt(Vector3 targetAbsRef, Vector3 upRelRef)
    {

        //Vector v1 = new Vector3(0, 0, -1);
        //Vector moveV = _staticModel.Position - vector;
        //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

        /*Vector forward = lookAt.Normalized();
        Vector right = Vector::Cross(up.Normalized(), forward);
        Vector up = Vector::Cross(forward, right);*/
        Vector3 forward;
        if (IsChild)
        {
            //Normalizing target and transforming it to local system
            forward = Geometric.Quaternion_Rotate(_parentModel.Orientation.Inverted(), targetAbsRef.Normalized()) - _parentModel.Position;
        }
        else
        {
            //Normalizing target.. local system is the same as world system
            forward = targetAbsRef.Normalized();
        }
        //Normalizing upVector (we are assuming it is expressed in local system)
        Vector3 eye = _position.Normalized();
        Vector3 up = upRelRef.Normalized();

        //Insert manual imprecision to avoid singularity
        if (Vector3.Dot(forward, up) == 1)
        {
            forward.X += 0.001f;
            forward.Y += 0.001f;
            forward.Z += 0.001f;
        }

        //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
        //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
        //Vector3 right = Vector3.CrossProduct(forward, up);

        Matrix4 lookAt_result = Matrix4.LookAt(eye.X, eye.Y, eye.Z, forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z);
        Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
        Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

        /*
        Quaternion targetRelOrientation_quaternion = new Quaternion();
        targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
        float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
        targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
        targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
        targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
        */

        _orientation = targetRelOrientation_quaternion;

    }

    public void ConstraintLookAt(Vector3 targetAbsRef, Vector3 upRelRef, Vector3 constraintAxisSelect)
    {

        Vector3 targetRelRef;

        if (IsChild)
            targetRelRef = Geometric.Quaternion_Rotate(_parentModel.Orientation.Inverted(), targetAbsRef);
        else
            targetRelRef = targetAbsRef;

        if (constraintAxisSelect.X != 0)
            targetRelRef.X = 0;
        else if (constraintAxisSelect.Y != 0)
            targetRelRef.Y = 0;
        else if (constraintAxisSelect.Z != 0)
            targetRelRef.Z = 0;

        //Vector v1 = new Vector3(0, 0, -1);
        //Vector moveV = _staticModel.Position - vector;
        //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

        /*Vector forward = lookAt.Normalized();
        Vector right = Vector::Cross(up.Normalized(), forward);
        Vector up = Vector::Cross(forward, right);*/
        Vector3 forward;
        if (IsChild)
        {
            //Normalizing target and transforming it to local system
            forward = targetRelRef.Normalized() - _parentModel.Position;
        }
        else
        {
            //Normalizing target.. local system is the same as world system
            forward = targetRelRef.Normalized();
        }
        //Normalizing upVector (we are assuming it is expressed in local system)
        Vector3 eye = _position.Normalized();
        Vector3 up = upRelRef.Normalized();

        //Insert manual imprecision to avoid singularity
        if (Vector3.Dot(forward, up) == 1)
        {
            forward.X += 0.001f;
            forward.Y += 0.001f;
            forward.Z += 0.001f;
        }

        //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
        //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
        //Vector3 right = Vector3.CrossProduct(forward, up);

        Matrix4 lookAt_result = Matrix4.LookAt(eye.X, eye.Y, eye.Z, forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z);
        Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
        Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

        /*
        Quaternion targetRelOrientation_quaternion = new Quaternion();
        targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
        float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
        targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
        targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
        targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
        */

        _orientation = targetRelOrientation_quaternion;

    }

  }
}