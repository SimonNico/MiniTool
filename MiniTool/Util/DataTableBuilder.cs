using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
using System.Linq;
namespace MiniTool
{
    internal class DataTableBuilder<T>
    {
        private static readonly MethodInfo GetValueMethod = typeof(DataRow).GetMethod("get_Item", new[]
        {
            typeof(int)
        });

        private static readonly MethodInfo IsDbNullMethod = typeof(DataRow).GetMethod("IsNull", new[]
        {
            typeof(int)
        });

        private delegate T Load(DataRow dataRecord);

        private Load _handler;

        private DataTableBuilder()
        {
        }

        public T Build(DataRow dataRecord)
        {
            return _handler(dataRecord);
        }

        public static DataTableBuilder<T> CreateBuilder(DataRow dataRecord)
        {
            DynamicMethod methodCreateEntity = new DynamicMethod("DynamicCreateEntity", typeof(T), new[]
            {
                typeof(DataRow)
            }, typeof(T), true);
            var generator = methodCreateEntity.GetILGenerator();
            var result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            var propertyinfos = typeof(T).GetProperties();
            for (int i = 0; i < dataRecord.ItemArray.Length; i++)
            {
                var propertyInfo = propertyinfos.Where(p=>dataRecord.Table.Columns[i].ColumnName.Replace("_","").ToLower().Equals(p.Name.ToLower())).FirstOrDefault();
                var endIfLabel = generator.DefineLabel();
                if (propertyInfo == null || propertyInfo.GetSetMethod() == null)
                {
                    continue;
                }

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Callvirt, IsDbNullMethod);
                generator.Emit(OpCodes.Brtrue, endIfLabel);
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Callvirt, GetValueMethod);
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                generator.MarkLabel(endIfLabel);
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            return new DataTableBuilder<T>
            {
                _handler = (Load)methodCreateEntity.CreateDelegate(typeof(Load))
            };
        }
    }
}
