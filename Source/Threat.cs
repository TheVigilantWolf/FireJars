//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.Threat
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System.Diagnostics;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class Threat
//  {
//    public ITargetable TargetableObject;
//    public Formation Formation;
//    public Agent Agent;
//    public float ThreatValue;
//    public bool ForceTarget;

//    public override int GetHashCode() => base.GetHashCode();

//    public string Name
//    {
//      get
//      {
//        if (this.TargetableObject != null)
//          return this.TargetableObject.Entity().Name;
//        if (this.Agent != null)
//          return this.Agent.Name.ToString();
//        if (this.Formation != null)
//          return this.Formation.ToString();
//        TaleWorlds.Library.Debug.FailedAssert("Invalid threat", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", nameof (Name), 39);
//        return "Invalid";
//      }
//    }

//    public Vec3 TargetingPosition
//    {
//      get
//      {
//        if (this.TargetableObject != null)
//        {
//          (Vec3, Vec3) boundingBoxMinMax = this.TargetableObject.ComputeGlobalPhysicsBoundingBoxMinMax();
//          return (boundingBoxMinMax.Item2 + boundingBoxMinMax.Item1) * 0.5f + this.TargetableObject.GetTargetingOffset();
//        }
//        if (this.Agent != null)
//          return this.Agent.CollisionCapsuleCenter;
//        if (this.Formation != null)
//          return this.Formation.GetMedianAgent(false, false, this.Formation.GetAveragePositionOfUnits(false, false)).Position;
//        TaleWorlds.Library.Debug.FailedAssert("Invalid threat", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", nameof (TargetingPosition), 64);
//        return Vec3.Invalid;
//      }
//    }

//    public (Vec3, Vec3) ComputeGlobalTargetingBoundingBoxMinMax()
//    {
//      if (this.TargetableObject != null)
//      {
//        (Vec3 vec3_1, Vec3 vec3_2) = this.TargetableObject.ComputeGlobalPhysicsBoundingBoxMinMax();
//        return (vec3_1 + this.TargetableObject.GetTargetingOffset(), vec3_2 + this.TargetableObject.GetTargetingOffset());
//      }
//      if (this.Agent != null)
//        return this.Agent.CollisionCapsule.GetBoxMinMax();
//      if (this.Formation == null)
//        return (Vec3.Invalid, Vec3.Invalid);
//      TaleWorlds.Library.Debug.FailedAssert("Nobody should be requesting a bounding box for a formation", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Threat.cs", nameof (ComputeGlobalTargetingBoundingBoxMinMax), 83);
//      return (Vec3.Invalid, Vec3.Invalid);
//    }

//    public Vec3 GetGlobalVelocity()
//    {
//      if (this.TargetableObject != null)
//        return this.TargetableObject.GetTargetGlobalVelocity();
//      return this.Agent != null ? new Vec3(this.Agent.GetAverageRealGlobalVelocity().AsVec2) : Vec3.Zero;
//    }

//    public override bool Equals(object obj)
//    {
//      return obj is Threat threat && this.TargetableObject == threat.TargetableObject && this.Formation == threat.Formation;
//    }

//    [Conditional("DEBUG")]
//    public void DisplayDebugInfo()
//    {
//    }
//  }
//}
