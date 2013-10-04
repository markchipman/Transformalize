#region License
// /*
// See license included in this library folder.
// */
#endregion
#if !NO_LCG

#region Using Directives

using System;
using System.Reflection;
using System.Reflection.Emit;
using Transformalize.Libs.Ninject.Components;

#endregion

namespace Transformalize.Libs.Ninject.Injection
{
    /// <summary>
    ///     Creates injectors for members via <see cref="DynamicMethod" />s.
    /// </summary>
    public class DynamicMethodInjectorFactory : NinjectComponent, IInjectorFactory
    {
        /// <summary>
        ///     Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
#if SILVERLIGHT
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof(object), new[] { typeof(object[]) });
            #else
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof (object), new[] {typeof (object[])}, true);
#endif

            var il = dynamicMethod.GetILGenerator();

            EmitLoadMethodArguments(il, constructor);
            il.Emit(OpCodes.Newobj, constructor);

            if (constructor.ReflectedType.IsValueType)
                il.Emit(OpCodes.Box, constructor.ReflectedType);

            il.Emit(OpCodes.Ret);

            return (ConstructorInjector) dynamicMethod.CreateDelegate(typeof (ConstructorInjector));
        }

        /// <summary>
        ///     Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        public PropertyInjector Create(PropertyInfo property)
        {
#if NO_SKIP_VISIBILITY
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof(void), new[] { typeof(object), typeof(object) });
            #else
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof (void), new[] {typeof (object), typeof (object)}, true);
#endif

            var il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            EmitUnboxOrCast(il, property.DeclaringType);

            il.Emit(OpCodes.Ldarg_1);
            EmitUnboxOrCast(il, property.PropertyType);

#if !SILVERLIGHT
            var injectNonPublic = Settings.InjectNonPublic;
#else
            const bool injectNonPublic = false;
            #endif
            // !SILVERLIGHT

            EmitMethodCall(il, property.GetSetMethod(injectNonPublic));
            il.Emit(OpCodes.Ret);

            return (PropertyInjector) dynamicMethod.CreateDelegate(typeof (PropertyInjector));
        }

        /// <summary>
        ///     Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        public MethodInjector Create(MethodInfo method)
        {
#if NO_SKIP_VISIBILITY
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof(void), new[] { typeof(object), typeof(object[]) });
            #else
            var dynamicMethod = new DynamicMethod(GetAnonymousMethodName(), typeof (void), new[] {typeof (object), typeof (object[])}, true);
#endif

            var il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            EmitUnboxOrCast(il, method.DeclaringType);

            EmitLoadMethodArguments(il, method);
            EmitMethodCall(il, method);

            if (method.ReturnType != typeof (void))
                il.Emit(OpCodes.Pop);

            il.Emit(OpCodes.Ret);

            return (MethodInjector) dynamicMethod.CreateDelegate(typeof (MethodInjector));
        }

        private static void EmitLoadMethodArguments(ILGenerator il, MethodBase targetMethod)
        {
            var parameters = targetMethod.GetParameters();
            var ldargOpcode = targetMethod is ConstructorInfo ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1;

            for (var idx = 0; idx < parameters.Length; idx++)
            {
                il.Emit(ldargOpcode);
                il.Emit(OpCodes.Ldc_I4, idx);
                il.Emit(OpCodes.Ldelem_Ref);

                EmitUnboxOrCast(il, parameters[idx].ParameterType);
            }
        }

        private static void EmitMethodCall(ILGenerator il, MethodInfo method)
        {
            var opCode = method.IsFinal ? OpCodes.Call : OpCodes.Callvirt;
            il.Emit(opCode, method);
        }

        private static void EmitUnboxOrCast(ILGenerator il, Type type)
        {
            var opCode = type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass;
            il.Emit(opCode, type);
        }

        private static string GetAnonymousMethodName()
        {
            return "DynamicInjector" + Guid.NewGuid().ToString("N");
        }
    }
}

#endif
//!NO_LCG