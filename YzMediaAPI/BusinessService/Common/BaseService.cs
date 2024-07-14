using log4net;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace YzMediaAPI.BusinessService
{
	public abstract class BaseService<T> where T : BaseService<T>, new()
	{
		/// <summary>
		/// 	Logger
		/// </summary>
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static T _instance = default(T);

		public static T GetInstance()
		{
			if (_instance == null)
			{
				_instance = new T();
				_instance.ProcessAfterInitialize();
			}

			return _instance;
		}

		protected virtual void ProcessAfterInitialize()
		{
			//do nothing;
		}

		protected void WriteLogExecutingCommand(SqlCommand command)
		{
			StringBuilder builder = new StringBuilder("");
			builder.AppendLine("Preparing SQL command with parameters: ");
			foreach (SqlParameter param in command.Parameters)
			{
				if (param.Value == DBNull.Value)
				{
					builder.AppendLine($"declare {param.ParameterName} {param.SqlDbType}; set {param.ParameterName}=null;");
				}
				else
				{
					builder.AppendLine(string.Format("declare {0} {1} {3}; set {0}='{2}';",
						param.ParameterName,
						param.SqlDbType.ToString(),
						param.Value == DBNull.Value ? "null" : param.Value.ToString(), param.SqlDbType.ToString().ToLower().Contains("char") ? $"({param.Size})" : null));
				}
			}
			builder.AppendLine("--SQL executing: ");
			builder.AppendLine(command.CommandText);
			builder.AppendLine(string.Format("--on host {0} and db {1}", command.Connection != null ? command.Connection.DataSource : string.Empty,
				command.Connection != null ? command.Connection.Database : string.Empty));
			_logger.Debug(builder.ToString());
		}

		protected object GetObjectValue(SqlDataReader reader, string keyProperty)
		{
			if (!System.DBNull.Value.Equals(reader[keyProperty]) && reader[keyProperty] != null)
			{
				return (object)reader[keyProperty];
			}

			return null;
		}

		protected object GetDatabaseValue(object value)
		{
			if (value is string)
			{
				return string.IsNullOrEmpty((string)value) ? System.DBNull.Value : value;
			}

			return value == null ? System.DBNull.Value : value;
		}

		protected K GetDbReaderValue<K>(object value)
		{
			if (System.DBNull.Value.Equals(value) == false && value != null)
			{
				return (K)value;
			}

			return default(K);
		}

		protected void ApplyValue(object dataObject, object value, string valueProperty)
		{
			if (System.DBNull.Value.Equals(value) == false)
			{
				PropertyInfo propertyInfo = dataObject.GetType().GetProperty(valueProperty);
				if (propertyInfo != null)
				{
					propertyInfo.SetValue(dataObject, value, null);
				}
			}
		}

		protected string FormatListData(List<int> listValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var value in listValues)
			{
				stringBuilder.AppendFormat(", {0}", value);
			}

			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 2);
			}

			return stringBuilder.ToString();
		}

		protected string FormatListData(List<string> listValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var value in listValues)
			{
				stringBuilder.AppendFormat(", '{0}'", CommonCoreUtils.ConvertString(value?.Trim()));
			}

			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 2);
			}
			else
			{
				stringBuilder.AppendFormat("''");
			}

			return stringBuilder.ToString();
		}

		protected void AddSqlParameter(SqlCommand command, string parameterName, object value, System.Data.SqlDbType type, int size = 0)
		{
			SqlParameter parameter = null;
			if (size > 0)
			{
				parameter = new SqlParameter(parameterName, type, size);
			}
			else
			{
				parameter = new SqlParameter(parameterName, type);
			}

			parameter.Value = GetDatabaseValue(value);
			command.Parameters.Add(parameter);
		}

		protected DataSet ToDataSet<K>(IList<K> list)
		{
			Type elementType = typeof(K);
			DataSet ds = new DataSet();
			DataTable t = new DataTable();
			ds.Tables.Add(t);

			//add a column to table for each public property on T
			foreach (var propInfo in elementType.GetProperties())
			{
				Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

				t.Columns.Add(propInfo.Name, ColType);
			}

			//go through each property on T and add each value to the table
			foreach (K item in list)
			{
				DataRow row = t.NewRow();

				foreach (var propInfo in elementType.GetProperties())
				{
					row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
				}

				t.Rows.Add(row);
			}

			return ds;
		}
	}

	public abstract class BaseMapInstance<T> where T : BaseMapInstance<T>, new()
	{
		/// <summary>
		/// 	Logger
		/// </summary>
		protected ILog _logger = null;

		private static readonly ILog _internalLogger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected static Dictionary<string, T> _mapInstance = new Dictionary<string, T>();

		protected static AutoResetEvent _lockExclusiveAccess = new AutoResetEvent(true); //initial unlock

		public BaseMapInstance()
		{
			_logger = LogManager.GetLogger(this.GetType());
		}

		public static T GetInstance(string idObject)
		{
			try
			{
				_lockExclusiveAccess.WaitOne();

				T instance = default(T);
				if (!_mapInstance.TryGetValue(idObject, out instance))
				{
					instance = new T();
					_mapInstance.Add(idObject, instance);
				}

				return instance;
			}
			catch (Exception ex)
			{
				_internalLogger.Error(ex.Message, ex);
				throw;
			}
			finally
			{
				_lockExclusiveAccess.Set();
			}
		}
	}

}
