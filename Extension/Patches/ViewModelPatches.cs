// using System;
// using System.Reflection;
// using HarmonyLib;
// using TaleWorlds.Library;
// using UIExtenderLibModule.ViewModel;
// using YAPO.Global;
// using YAPO.ViewModels;
//
// // ReSharper disable InconsistentNaming
// // ReSharper disable ParameterTypeCanBeEnumerable.Global
// // ReSharper disable RedundantAssignment
// // ReSharper disable UnusedType.Global
// // ReSharper disable UnusedMember.Global
//
// namespace YAPO.Patches
// {
//     public class ViewModelPatches
//     {
//         [HarmonyPatch(typeof(ViewModel), "ExecuteCommand")]
//         public static class ViewModelExecuteCommandCallsite
//         {
//             public static void Postfix(ViewModel __instance, string commandName, object[] parameters)
//             {
//                 Type type = __instance.GetType();
//
//                 if (commandName == nameof(PartyVmMixin.ChangeOrderOfTypeOption))
//                 {
//                     States.PartyVmMixin.ChangeOrderOfTypeOption((TypeSortOptionVm) parameters[0], (int) parameters[1], (string) parameters[2]);
//                 }
//
//                 MethodInfo method = ViewModelPatchUtil.FindExecuteCommandMethod(type,
//                                                                                 commandName,
//                                                                                 BindingFlags.Instance |
//                                                                                 BindingFlags.Public |
//                                                                                 BindingFlags.NonPublic);
//                 if (!(method != null))
//                 {
//                     return;
//                 }
//
//                 if (method.GetParameters().Length == parameters.Length)
//                 {
//                     object[] objArray = new object[parameters.Length];
//                     ParameterInfo[] parameters1 = method.GetParameters();
//
//                     MethodInfo convertValueToMethod =
//                         typeof(ViewModel).GetMethod("ConvertValueTo", BindingFlags.Static | BindingFlags.NonPublic);
//                     if (convertValueToMethod == null)
//                     {
//                         throw new Exception("Cannot find ConvertValueTo method of ViewModel");
//                     }
//
//                     for (int index = 0; index < parameters.Length; ++index)
//                     {
//                         object parameter = parameters[index];
//                         Type parameterType = parameters1[index].ParameterType;
//                         objArray[index] = parameter;
//                         if (!(parameter is string) || parameterType == typeof(string)) continue;
//
//                         object obj = convertValueToMethod.Invoke(__instance, new[] {parameter, parameterType});
//                         objArray[index] = obj;
//                     }
//
//                     method.InvokeWithLog(__instance, objArray);
//                 }
//                 else
//                 {
//                     if (method.GetParameters().Length != 0)
//                     {
//                         return;
//                     }
//
//                     method.InvokeWithLog(__instance);
//                 }
//             }
//         }
//     }
// }
