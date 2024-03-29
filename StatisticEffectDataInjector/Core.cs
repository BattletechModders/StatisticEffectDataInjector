﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Cil;

namespace StatisticEffectDataInjector {
  internal static class Injector {
    internal static MethodReference HBS_Util_Serialization_StorageSpaceString { get; set; } = null;
    internal static MethodReference HBS_Util_SerializationStream_PutString { get; set; } = null;
    internal static MethodReference HBS_Util_SerializationStream_GetString { get; set; } = null;
    internal static MethodReference HBS_Util_SerializationStream_PutBool { get; set; } = null;
    internal static MethodReference HBS_Util_SerializationStream_GetBool { get; set; } = null;
    internal static FieldReference HBS_Util_Serialization_STORAGE_SPACE_BOOL { get; set; } = null;
    public static void InjectSize(TypeDefinition StatisticEffectDataType, FieldDefinition field) {
      MethodDefinition sizeMethod = StatisticEffectDataType.Methods.First(x => x.Name == "Size");
      if (sizeMethod == null) {
        Log.Error?.WL(1, "can't find BattleTech.StatisticEffectData.Size method");
        return;
      }
      Instruction targetInstruction = null;
      for (var i = 0; i < sizeMethod.Body.Instructions.Count; i++) {
        var instruction = sizeMethod.Body.Instructions[i];
        if (instruction.OpCode == OpCodes.Ret) { targetInstruction = sizeMethod.Body.Instructions[i - 1]; }

      }
      if (targetInstruction == null) {
        Log.Error?.WL(1, "can't find return opcode");
        return;
      }
      //IL_00b6: ldarg.0      // this
      //IL_00b7: ldfld        string BattleTech.StatisticEffectData::targetAmmoCategoryID
      //IL_00bc: call int32 HBS.Util.Serialization::StorageSpaceString(string)
      //IL_00c1: add
      ILProcessor body = sizeMethod.Body.GetILProcessor();
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Add));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Call, HBS_Util_Serialization_StorageSpaceString));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldfld, field));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldarg_0));
      Log.Debug?.TWL(0, $"InjectSize {field.Name} success");
      for (var i = 0; i < sizeMethod.Body.Instructions.Count; i++) {
        var instruction = sizeMethod.Body.Instructions[i];
        Log.Debug?.WL(1, instruction.OpCode + ":" + (instruction.Operand == null ? "null" : instruction.Operand.ToString()));
      }
      Log.Debug?.WL(0, $"method end");
    }
    public static void InjectSave(TypeDefinition StatisticEffectDataType, FieldDefinition field) {
      MethodDefinition saveMethod = StatisticEffectDataType.Methods.First(x => x.Name == "Save");
      if (saveMethod == null) {
        Log.Error?.WL(1, "can't find BattleTech.StatisticEffectData.Size method");
        return;
      }
      Instruction targetInstruction = null;
      for (var i = 0; i < saveMethod.Body.Instructions.Count; i++) {
        var instruction = saveMethod.Body.Instructions[i];
        if (instruction.OpCode == OpCodes.Ret) { targetInstruction = saveMethod.Body.Instructions[i - 1]; }

      }
      if (targetInstruction == null) {
        Log.Error?.WL(1, "can't find return opcode");
        return;
      }
      //IL_00c1: ldarg.1      // 'stream'
      //IL_00c2: ldarg.0      // this
      //IL_00c3: ldfld        string BattleTech.StatisticEffectData::targetAmmoCategoryID
      //IL_00c8: callvirt instance void HBS.Util.SerializationStream::PutString(string)
      ILProcessor body = saveMethod.Body.GetILProcessor();
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Callvirt, HBS_Util_SerializationStream_PutString));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldfld, field));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldarg_0));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldarg_1));
      Log.Debug?.TWL(0, $"InjectSave {field.Name} success");
      for (var i = 0; i < saveMethod.Body.Instructions.Count; i++) {
        var instruction = saveMethod.Body.Instructions[i];
        Log.Debug?.WL(1, instruction.OpCode + ":" + (instruction.Operand == null ? "null" : instruction.Operand.ToString()));
      }
      Log.Debug?.WL(0, $"method end");
    }
    public static void InjectLoad(TypeDefinition StatisticEffectDataType, FieldDefinition field) {
      MethodDefinition loadMethod = StatisticEffectDataType.Methods.First(x => x.Name == "Load");
      if (loadMethod == null) {
        Log.Error?.WL(1, "can't find BattleTech.StatisticEffectData.Size method");
        return;
      }
      Instruction targetInstruction = null;
      for (var i = 0; i < loadMethod.Body.Instructions.Count; i++) {
        var instruction = loadMethod.Body.Instructions[i];
        if (instruction.OpCode == OpCodes.Ret) { targetInstruction = loadMethod.Body.Instructions[i - 3]; }
      }
      if (targetInstruction == null) {
        Log.Error?.WL(1, "can't find return opcode");
        return;
      }
      //IL_00e8: ldarg.0      // this
      //IL_00e9: ldarg.1      // 'stream'
      //IL_00ea: callvirt instance string HBS.Util.SerializationStream::GetString()
      //IL_00ef: stfld        string BattleTech.StatisticEffectData::targetAmmoCategoryID
      ILProcessor body = loadMethod.Body.GetILProcessor();
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Stfld, field));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Callvirt, HBS_Util_SerializationStream_GetString));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldarg_1));
      body.InsertAfter(targetInstruction, Instruction.Create(OpCodes.Ldarg_0));
      Log.Debug?.TWL(0, $"InjectLoad {field.Name} success");
      for (var i = 0; i < loadMethod.Body.Instructions.Count; i++) {
        var instruction = loadMethod.Body.Instructions[i];
        Log.Debug?.WL(1, instruction.OpCode + ":" + (instruction.Operand == null ? "null" : instruction.Operand.ToString()));
      }
      Log.Debug?.WL(0, $"method end");
    }

    public static void InjectStatisticEffectMethod(MethodDefinition patchingMethod) {
      while (true) {
        int ti = -1;
        for (var i = 0; i < patchingMethod.Body.Instructions.Count - 3; i++) {
          var instruction = patchingMethod.Body.Instructions[i];
          if (instruction.OpCode != OpCodes.Ldfld) { continue; }
          if (instruction.Operand != game.MainModule.ImportReference(game.MainModule.GetType("BattleTech.Effect").Fields.First(x => x.Name == "effectData"))) { continue; }
          instruction = patchingMethod.Body.Instructions[i + 1];
          if (instruction.OpCode != OpCodes.Ldfld) { continue; }
          if (instruction.Operand != game.MainModule.ImportReference(game.MainModule.GetType("BattleTech.EffectData").Fields.First(x => x.Name == "statisticData"))) { continue; }
          instruction = patchingMethod.Body.Instructions[i + 2];
          if (instruction.OpCode != OpCodes.Ldfld) { continue; }
          if (instruction.Operand != game.MainModule.ImportReference(game.MainModule.GetType("BattleTech.StatisticEffectData").Fields.First(x => x.Name == "statName"))) { continue; }
          ti = i; break;
        }
        if (ti == -1) {
          Log.Error?.WL(0, $"Can't find effectData.statisticData.statName in {patchingMethod.Name}");
          break;
        }
        Log.Debug?.WL(0, $"found effectData.statisticData.statName in {patchingMethod.Name}. Replacing with modVariant.statName");
        var body = patchingMethod.Body.GetILProcessor();
        body.Replace(patchingMethod.Body.Instructions[ti + 0], body.Create(OpCodes.Ldfld, game.MainModule.ImportReference(game.MainModule.GetType("BattleTech.StatisticEffect").Fields.First(x => x.Name == "modVariant"))));
        body.Replace(patchingMethod.Body.Instructions[ti + 1], body.Create(OpCodes.Ldfld, game.MainModule.ImportReference(game.MainModule.GetType("BattleTech.Variant").Fields.First(x => x.Name == "statName"))));
        body.Replace(patchingMethod.Body.Instructions[ti + 2], body.Create(OpCodes.Nop));
      }
    }
    internal static AssemblyDefinition game { get; set; } = null;
    public static void Inject(IAssemblyResolver resolver) {
      Log.Error?.TWL(0, $"StatisticEffectDataInjector initing {Assembly.GetExecutingAssembly().GetName().Version}");
      try {
        game = resolver.Resolve(new AssemblyNameReference("Assembly-CSharp", null));
        if (game == null) {
          Log.Error?.WL(1, "can't resolve main game assembly");
          return;
        }
        HBS_Util_Serialization_StorageSpaceString = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.Serialization").Methods.First(x => x.Name == "StorageSpaceString"));
        HBS_Util_SerializationStream_PutString = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.SerializationStream").Methods.First(x => x.Name == "PutString"));
        HBS_Util_SerializationStream_GetString = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.SerializationStream").Methods.First(x => x.Name == "GetString"));
        HBS_Util_SerializationStream_PutBool = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.SerializationStream").Methods.First(x => x.Name == "PutBool"));
        HBS_Util_SerializationStream_GetBool = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.SerializationStream").Methods.First(x => x.Name == "GetBool"));
        HBS_Util_Serialization_STORAGE_SPACE_BOOL = game.MainModule.ImportReference(game.MainModule.GetType("HBS.Util.Serialization").Fields.First(x => x.Name == "STORAGE_SPACE_BOOL"));

        TypeDefinition StatisticEffectDataType = game.MainModule.GetType("BattleTech.StatisticEffectData");
        if (StatisticEffectDataType == null) {
          Log.Error?.WL(1, "can't resolve BattleTech.StatisticEffectData type");
          return;
        }
        Log.Debug?.WL(1, "fields before:");
        foreach (var field in StatisticEffectDataType.Fields) {
          Log.Debug?.WL(2, $"{field.Name}");
        }
        FieldDefinition statNameFieldDef = StatisticEffectDataType.Fields.First(x => x.Name == "statName");
        if (statNameFieldDef == null) {
          Log.Error?.WL(1, "can't find BattleTech.StatisticEffectData.statName field");
          return;
        }
        List<CustomAttribute> statName_attrs = statNameFieldDef.HasCustomAttributes ? statNameFieldDef.CustomAttributes.ToList() : new List<CustomAttribute>();

        FieldDefinition LocationFieldDef = new FieldDefinition("Location", Mono.Cecil.FieldAttributes.Public, game.MainModule.ImportReference(typeof(string)));
        FieldDefinition ShouldHaveTagsFieldDef = new FieldDefinition("ShouldHaveTags", Mono.Cecil.FieldAttributes.Public, game.MainModule.ImportReference(typeof(string)));
        FieldDefinition ShouldNotHaveTagsFieldDef = new FieldDefinition("ShouldNotHaveTags", Mono.Cecil.FieldAttributes.Public, game.MainModule.ImportReference(typeof(string)));
        Log.Debug?.WL(1, $"BattleTech.StatisticEffectData.statName custom attributes {statName_attrs.Count}:");
        foreach (var attr in statName_attrs) {
          LocationFieldDef.CustomAttributes.Add(attr);
          ShouldHaveTagsFieldDef.CustomAttributes.Add(attr);
          ShouldNotHaveTagsFieldDef.CustomAttributes.Add(attr);
          Log.Debug?.WL(2, $"{attr.AttributeType.Name}");
        }
        StatisticEffectDataType.Fields.Add(LocationFieldDef);
        StatisticEffectDataType.Fields.Add(ShouldHaveTagsFieldDef);
        StatisticEffectDataType.Fields.Add(ShouldNotHaveTagsFieldDef);
        Log.Debug?.WL(1, "fields after:");
        foreach (var field in StatisticEffectDataType.Fields) {
          Log.Debug?.WL(2, $"{field.Name}");
        }
        Log.Debug?.WL(1, "field added successfully");

        InjectSize(StatisticEffectDataType, LocationFieldDef);
        InjectSave(StatisticEffectDataType, LocationFieldDef);
        InjectLoad(StatisticEffectDataType, LocationFieldDef);

        InjectSize(StatisticEffectDataType, ShouldHaveTagsFieldDef);
        InjectSave(StatisticEffectDataType, ShouldHaveTagsFieldDef);
        InjectLoad(StatisticEffectDataType, ShouldHaveTagsFieldDef);

        InjectSize(StatisticEffectDataType, ShouldNotHaveTagsFieldDef);
        InjectSave(StatisticEffectDataType, ShouldNotHaveTagsFieldDef);
        InjectLoad(StatisticEffectDataType, ShouldNotHaveTagsFieldDef);

        InjectStatisticEffectMethod(game.MainModule.GetType("BattleTech.StatisticEffect").Methods.First(x => x.Name == "OnEffectBegin"));
        InjectStatisticEffectMethod(game.MainModule.GetType("BattleTech.StatisticEffect").Methods.First(x => x.Name == "OnEffectPhaseBegin"));
        InjectStatisticEffectMethod(game.MainModule.GetType("BattleTech.StatisticEffect").Methods.First(x => x.Name == "OnEffectActivationEnd"));
        InjectStatisticEffectMethod(game.MainModule.GetType("BattleTech.StatisticEffect").Methods.First(x => x.Name == "OnEffectEnd"));
        InjectStatisticEffectMethod(game.MainModule.GetType("BattleTech.StatisticEffect").Methods.First(x => x.Name == "OnEffectTakeDamage"));

        TypeDefinition EffectDurationData = game.MainModule.GetType("BattleTech.EffectDurationData");
        if (EffectDurationData == null) {
          Log.Error?.WL(1, "can't resolve BattleTech.EffectDurationData type");
          return;
        }
        Log.Debug?.WL(1, "fields before:");
        foreach (var field in EffectDurationData.Fields) {
          Log.Debug?.WL(2, $"{field.Name}");
        }
        FieldDefinition duration = EffectDurationData.Fields.First(x => x.Name == "duration");
        if (duration == null) {
          Log.Error?.WL(1, "can't find BattleTech.EffectDurationData.duration field");
          return;
        }
        List<CustomAttribute> duration_attrs = statNameFieldDef.HasCustomAttributes ? statNameFieldDef.CustomAttributes.ToList() : new List<CustomAttribute>();
        FieldDefinition stackId = new FieldDefinition("stackId", Mono.Cecil.FieldAttributes.Public, game.MainModule.ImportReference(typeof(string)));
        foreach (var attr in duration_attrs) {
          stackId.CustomAttributes.Add(attr);
          Log.Debug?.WL(2, $"{attr.AttributeType.Name}");
        }
        EffectDurationData.Fields.Add(stackId);
        Log.Debug?.WL(1, "fields after:");
        foreach (var field in EffectDurationData.Fields) {
          Log.Debug?.WL(2, $"{field.Name}");
        }
        Log.Debug?.WL(1, "field added successfully");
        InjectSize(EffectDurationData, stackId);
        InjectSave(EffectDurationData, stackId);
        InjectLoad(EffectDurationData, stackId);
      } catch (Exception e) {
        Log.Error?.TWL(0, e.ToString());
      }
    }
  }
}
