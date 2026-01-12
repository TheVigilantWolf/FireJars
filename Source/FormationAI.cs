//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.MountAndBlade.FormationAI
//// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: F9CDD251-473F-4DD9-95A1-FA6F3C219515
//// Assembly location: C:\Users\trist\Documents\Modding\Bannerlord\DLLs\TaleWorlds.MountAndBlade.dll

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using TaleWorlds.Core;
//using TaleWorlds.Engine;
//using TaleWorlds.Library;

//#nullable disable
//namespace TaleWorlds.MountAndBlade
//{
//  public class FormationAI
//  {
//    private const float BehaviorPreserveTime = 5f;
//    private readonly Formation _formation;
//    private readonly List<FormationAI.BehaviorData> _specialBehaviorData;
//    private readonly List<BehaviorComponent> _behaviors = new List<BehaviorComponent>();
//    private BehaviorComponent _activeBehavior;
//    private FormationAI.BehaviorSide _side = FormationAI.BehaviorSide.Middle;
//    private readonly Timer _tickTimer;

//    public event Action<Formation> OnActiveBehaviorChanged;

//    public BehaviorComponent ActiveBehavior
//    {
//      get => this._activeBehavior;
//      private set
//      {
//        if (this._activeBehavior == value)
//          return;
//        this._activeBehavior?.OnBehaviorCanceled();
//        BehaviorComponent activeBehavior = this._activeBehavior;
//        this._activeBehavior = value;
//        this._activeBehavior.OnBehaviorActivated();
//        this.ActiveBehavior.PreserveExpireTime = Mission.Current.CurrentTime + 10f;
//        if (this.OnActiveBehaviorChanged == null || activeBehavior != null && activeBehavior.Equals((object) value))
//          return;
//        this.OnActiveBehaviorChanged(this._formation);
//      }
//    }

//    public FormationAI.BehaviorSide Side
//    {
//      get => this._side;
//      set
//      {
//        if (this._side == value)
//          return;
//        this._side = value;
//        if (this._side == FormationAI.BehaviorSide.BehaviorSideNotSet)
//          return;
//        foreach (BehaviorComponent behavior in this._behaviors)
//          behavior.OnValidBehaviorSideChanged();
//      }
//    }

//    public bool IsMainFormation { get; set; }

//    public int BehaviorCount => this._behaviors.Count;

//    public FormationAI(Formation formation)
//    {
//      this._formation = formation;
//      float num1 = 0.0f;
//      if (formation.Team != null)
//      {
//        double num2 = 0.10000000149011612 * (double) formation.FormationIndex;
//        float num3 = 0.0f;
//        if (formation.Team.TeamIndex >= 0)
//          num3 = (float) ((double) formation.Team.TeamIndex * 0.5 * 0.10000000149011612);
//        double num4 = (double) num3;
//        num1 = (float) (num2 + num4);
//      }
//      this._tickTimer = new Timer(Mission.Current.CurrentTime + 0.5f * num1, 0.5f);
//      this._specialBehaviorData = new List<FormationAI.BehaviorData>();
//    }

//    public T SetBehaviorWeight<T>(float w) where T : BehaviorComponent
//    {
//      foreach (BehaviorComponent behavior in this._behaviors)
//      {
//        if (behavior is T obj)
//        {
//          obj.WeightFactor = w;
//          return obj;
//        }
//      }
//      throw new MBException("Behavior weight could not be set.");
//    }

//    public void AddAiBehavior(BehaviorComponent behaviorComponent)
//    {
//      this._behaviors.Add(behaviorComponent);
//    }

//    public T GetBehavior<T>() where T : BehaviorComponent
//    {
//      foreach (BehaviorComponent behavior1 in this._behaviors)
//      {
//        if (behavior1 is T behavior2)
//          return behavior2;
//      }
//      foreach (FormationAI.BehaviorData behaviorData in this._specialBehaviorData)
//      {
//        if (behaviorData.Behavior is T behavior)
//          return behavior;
//      }
//      return default (T);
//    }

//    public void AddSpecialBehavior(BehaviorComponent behavior, bool purgePreviousSpecialBehaviors = false)
//    {
//      if (purgePreviousSpecialBehaviors)
//        this._specialBehaviorData.Clear();
//      this._specialBehaviorData.Add(new FormationAI.BehaviorData()
//      {
//        Behavior = behavior
//      });
//    }

//    private bool FindBestBehavior()
//    {
//      BehaviorComponent behaviorComponent = (BehaviorComponent) null;
//      float num1 = float.MinValue;
//      foreach (BehaviorComponent behavior in this._behaviors)
//      {
//        if ((double) behavior.WeightFactor > 1.0000000116860974E-07)
//        {
//          float num2 = behavior.GetAIWeight() * behavior.WeightFactor;
//          if (behavior == this.ActiveBehavior)
//            num2 *= MBMath.Lerp(1.2f, 2f, MBMath.ClampFloat((float) (((double) behavior.PreserveExpireTime - (double) Mission.Current.CurrentTime) / 5.0), 0.0f, 1f), float.MinValue);
//          if ((double) num2 > (double) num1)
//          {
//            if ((double) behavior.NavmeshlessTargetPositionPenalty > 0.0)
//              num2 /= behavior.NavmeshlessTargetPositionPenalty;
//            behavior.PrecalculateMovementOrder();
//            float num3 = num2 * behavior.NavmeshlessTargetPositionPenalty;
//            if ((double) num3 > (double) num1)
//            {
//              behaviorComponent = behavior;
//              num1 = num3;
//            }
//          }
//        }
//      }
//      if (behaviorComponent == null)
//        return false;
//      this.ActiveBehavior = behaviorComponent;
//      if (behaviorComponent != this._behaviors[0])
//      {
//        this._behaviors.Remove(behaviorComponent);
//        this._behaviors.Insert(0, behaviorComponent);
//      }
//      return true;
//    }

//    private void PreprocessBehaviors()
//    {
//      if (!this._formation.HasAnyEnemyFormationsThatIsNotEmpty())
//        return;
//      FormationAI.BehaviorData behaviorData = this._specialBehaviorData.FirstOrDefault<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (sd => !sd.IsPreprocessed));
//      if (behaviorData == null)
//        return;
//      behaviorData.Behavior.TickOccasionally();
//      float aiWeight = behaviorData.Behavior.GetAIWeight();
//      if (behaviorData.Behavior == this.ActiveBehavior)
//        aiWeight *= MBMath.Lerp(1.01f, 1.5f, MBMath.ClampFloat((float) (((double) behaviorData.Behavior.PreserveExpireTime - (double) Mission.Current.CurrentTime) / 5.0), 0.0f, 1f), float.MinValue);
//      behaviorData.Weight = aiWeight * behaviorData.Preference;
//      behaviorData.IsPreprocessed = true;
//    }

//    public void Tick()
//    {
//      if (!Mission.Current.AllowAiTicking || !Mission.Current.ForceTickOccasionally && !this._tickTimer.Check(Mission.Current.CurrentTime))
//        return;
//      this.TickOccasionally(this._tickTimer.PreviousDeltaTime);
//    }

//    private void TickOccasionally(float dt)
//    {
//      this._formation.IsAITickedAfterSplit = true;
//      if (this.FindBestBehavior())
//      {
//        if (!this._formation.IsAIControlled)
//        {
//          if (!GameNetwork.IsMultiplayer || Mission.Current.MainAgent == null || this._formation.Team.IsPlayerGeneral || !this._formation.Team.IsPlayerSergeant || this._formation.PlayerOwner != Agent.Main)
//            return;
//          this.ActiveBehavior.RemindSergeantPlayer();
//        }
//        else
//          this.ActiveBehavior.TickOccasionally();
//      }
//      else
//      {
//        BehaviorComponent behaviorComponent = this.ActiveBehavior;
//        if (!this._formation.HasAnyEnemyFormationsThatIsNotEmpty())
//          return;
//        this.PreprocessBehaviors();
//        foreach (FormationAI.BehaviorData behaviorData in this._specialBehaviorData)
//          behaviorData.IsPreprocessed = false;
//        if (behaviorComponent is BehaviorStop && this._specialBehaviorData.Count > 0)
//        {
//          IEnumerable<FormationAI.BehaviorData> source = this._specialBehaviorData.Where<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (sbd => (double) sbd.Weight > 0.0));
//          if (source.Any<FormationAI.BehaviorData>())
//            behaviorComponent = source.MaxBy<FormationAI.BehaviorData, float>((Func<FormationAI.BehaviorData, float>) (abd => abd.Weight)).Behavior;
//        }
//        int num = this._formation.IsAIControlled ? 1 : 0;
//        bool flag = false;
//        if (this.ActiveBehavior != behaviorComponent)
//        {
//          BehaviorComponent activeBehavior = this.ActiveBehavior;
//          this.ActiveBehavior = behaviorComponent;
//          flag = true;
//        }
//        if (!flag && (behaviorComponent == null || !behaviorComponent.IsCurrentOrderChanged))
//          return;
//        if (this._formation.IsAIControlled)
//          this._formation.SetMovementOrder(behaviorComponent.CurrentOrder);
//        behaviorComponent.IsCurrentOrderChanged = false;
//      }
//    }

//    public void OnDeploymentFinished()
//    {
//      foreach (BehaviorComponent behavior in this._behaviors)
//        behavior.OnDeploymentFinished();
//    }

//    public void OnAgentRemoved(Agent agent)
//    {
//      foreach (BehaviorComponent behavior in this._behaviors)
//        behavior.OnAgentRemoved(agent);
//    }

//    public BehaviorComponent GetBehaviorAtIndex(int index)
//    {
//      return index >= 0 && index < this._behaviors.Count ? this._behaviors[index] : (BehaviorComponent) null;
//    }

//    [Conditional("DEBUG")]
//    public void DebugMore()
//    {
//      if (!MBDebug.IsDisplayingHighLevelAI)
//        return;
//      foreach (FormationAI.BehaviorData behaviorData in (IEnumerable<FormationAI.BehaviorData>) this._specialBehaviorData.OrderBy<FormationAI.BehaviorData, string>((Func<FormationAI.BehaviorData, string>) (d => d.Behavior.GetType().ToString())))
//      {
//        behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "");
//        behaviorData.Weight.ToString("0.00");
//        behaviorData.Preference.ToString("0.00");
//      }
//    }

//    [Conditional("DEBUG")]
//    public void DebugScores()
//    {
//      if (this._formation.PhysicalClass.IsRanged())
//        MBDebug.Print("Ranged");
//      else if (this._formation.PhysicalClass.IsMeleeCavalry())
//        MBDebug.Print("Cavalry");
//      else
//        MBDebug.Print("Infantry");
//      foreach (FormationAI.BehaviorData behaviorData in (IEnumerable<FormationAI.BehaviorData>) this._specialBehaviorData.OrderBy<FormationAI.BehaviorData, string>((Func<FormationAI.BehaviorData, string>) (d => d.Behavior.GetType().ToString())))
//        MBDebug.Print(behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "") + " \t\t w:" + behaviorData.Weight.ToString("0.00") + "\t p:" + behaviorData.Preference.ToString("0.00"));
//    }

//    public void ResetBehaviorWeights()
//    {
//      foreach (BehaviorComponent behavior in this._behaviors)
//        behavior.ResetBehavior();
//    }

//    public class BehaviorData
//    {
//      public BehaviorComponent Behavior;
//      public float Preference = 1f;
//      public float Weight;
//      public bool IsRemovedOnCancel;
//      public bool IsPreprocessed;
//    }

//    public enum BehaviorSide
//    {
//      Left = 0,
//      Middle = 1,
//      Right = 2,
//      BehaviorSideNotSet = 3,
//      ValidBehaviorSideCount = 3,
//    }
//  }
//}
