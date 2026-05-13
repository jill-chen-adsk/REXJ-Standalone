using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for storage entity</summary>
    /// ================================================================================
    public class StorageUtility
    {
        /// ================================================================================
        /// <summary>Check exits GUID</summary>
        ///
        /// <param name="guid">GUID</param>
        /// <returns>True if exit, false otherwise</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static bool isExistGuid(Guid guid)
        {
            var schema = Autodesk.Revit.DB.ExtensibleStorage.Schema.Lookup(guid);
            if (schema == null)
                return false;

            return true;
        }

        /// ================================================================================
        /// <summary>Set value for field</summary>
        ///
        /// <param name="element">Element</param>
        /// <param name="guid">GUID</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="type">Type</param>
        /// <param name="value">Value</param>
        /// <returns>Return true if so, false otherwise</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static bool SetValue(Element element, Guid guid, string fieldName, Type type, object value)
        {
            if (element == null)
                return false;

            try
            {
                var schema = Autodesk.Revit.DB.ExtensibleStorage.Schema.Lookup(guid);
                if (schema == null)
                    return false;

                var entity = element.GetEntity(schema);
                if (entity == null || entity.Schema == null)
                    return false;

                if (type == typeof(IList<ElementId>))
                {
                    var list = (IList<ElementId>)value;
                    entity.Set<IList<ElementId>>(fieldName, list);
                }
                else if (type == typeof(string))
                {
                    var str = (string)value;
                    entity.Set<string>(fieldName, str);
                }
                else if (type == typeof(ElementId))
                {
                    var id = (ElementId)value;
                    entity.Set<ElementId>(fieldName, id);
                }
                else if (type == typeof(int))
                {
                    var iValue = (int)value;
                    entity.Set<int>(fieldName, iValue);
                }

                element.Document.Regenerate();

                element.SetEntity(entity);

                return true;
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>Get field value</summary>
        ///
        /// <param name="element">element</param>
        /// <param name="guidstring">guid</param>
        /// <param name="fieldName">field name</param>
        /// <param name="type">type of value</param>
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static object GetValue(Element element, string guidstring, string fieldName, Type type)
        {
            if (element == null)
                return null;
            try
            {
                Guid guid = new Guid(guidstring);
                if (guid == null)
                    return null;

                var schema = Autodesk.Revit.DB.ExtensibleStorage.Schema.Lookup(guid);
                if (schema == null)
                    return null;

                var entity = element.GetEntity(schema);
                if (entity == null || entity.Schema == null)
                    return null;

                object value = null;
                if (type == typeof(int))
                    value = entity.Get<int>(fieldName);
                else if (type == typeof(string))
                    value = entity.Get<string>(fieldName);
                else if (type == typeof(ElementId))
                    value = entity.Get<ElementId>(fieldName);
                else if (type == typeof(double))
                    value = entity.Get<double>(fieldName);
                else if (type == typeof(IList<ElementId>))
                    value = entity.Get<IList<ElementId>>(fieldName);

                return value;
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                return null;
            }
        }

        /// ================================================================================
        /// <summary>Set Extensible Storage</summary>
        ///
        /// <param name="elem">Element</param>
        /// <param name="nameSchema">name schema builder</param>
        /// <param name="nameVerId">name vendor id</param>
        /// <param name="nameField">field name</param>
        /// <param name="guidsettings">guid</param>
        /// <param name="value">string value</param>
        ///
        ///  <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static void SetExtensibleStorage(Element elem, string nameSchema, string nameVerId, string nameField, string guidsettings,
                                            string value)

        {
            Schema schema = CreateNewsSchema(elem, nameSchema, nameVerId, nameField, guidsettings);
            Guid guid = new Guid(guidsettings);
            if (schema != null
                && schema.GUID != Guid.Empty
                && schema.IsValidObject && guid != Guid.Empty)
            {
                SetValue(elem, guid, nameField, typeof(string), value);
            }
        }

        /// ================================================================================
        /// <summary>Create new Schema from predefined guid</summary>
        ///
        /// <param name="element">element</param>
        /// <param name="nameSchema">name Schema builder</param>
        /// <param name="nameVerdId">name vendor id</param>
        /// <param name="nameField">field name</param>
        /// <param name="setingGuid">guid</param>
        /// <returns>Schema</returns>
        ///
        ///  <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static Schema CreateNewsSchema(Element element, string nameSchema, string nameVerdId, string nameField, string setingGuid)
        {
            SchemaBuilder schemaBuilder = CreatePublicSchemaBuilder(nameSchema, nameVerdId, setingGuid);
            if (schemaBuilder == null)
                return null;

            try
            {
                FieldBuilder fielSetting = schemaBuilder.AddSimpleField(nameField, typeof(string));

                Schema schema = schemaBuilder.Finish();
                Entity entity = new Entity(schema);

                element.SetEntity(entity);

                return schema;
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                return null;
            }
        }

        /// ================================================================================
        /// <summary>Create base schema builder</summary>
        ///
        /// <param name="nameSchema"> name SchemaBuilder</param>
        /// <param name="nameVenddorId"> name vendon id</param>
        /// <param name="guidSetting"> guid</param>
        /// <returns>SchemaBuilder</returns>
        ///
        ///  <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static SchemaBuilder CreatePublicSchemaBuilder(string nameSchema, string nameVenddorId, string guidSetting)
        {
            try
            {
                Guid guid = new Guid(guidSetting);

                if (SchemaBuilder.GUIDIsValid(guid) == false)
                    return null;

                if (SchemaBuilder.VendorIdIsValid(nameVenddorId) == false)
                    return null;

                SchemaBuilder schemaBuilder = new SchemaBuilder(guid);

                if (schemaBuilder.AcceptableName(nameSchema) == false)
                    return null;

                schemaBuilder.SetVendorId(nameVenddorId);

                schemaBuilder.SetReadAccessLevel(AccessLevel.Public); // allow anyone to read the object
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public); // allow anyone to write the object
                schemaBuilder.SetSchemaName(nameSchema);

                return schemaBuilder;
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                return null;
            }
        }
    }
}