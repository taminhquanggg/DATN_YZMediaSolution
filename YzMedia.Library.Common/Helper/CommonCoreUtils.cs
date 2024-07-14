using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace YzMedia.Library.Common.Helper
{
	public static class CommonCoreUtils
	{
		/// <summary>
		/// 	Logger
		/// </summary>
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		//Ham kiem tra so
		public static bool IsNumeric(string text)
		{
			return string.IsNullOrEmpty(text) ? false :
					Regex.IsMatch(text, @"^\s*\-?\d+(\.\d+)?\s*$");
		}

		public static string FormatListData(List<string> listValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (listValues != null && listValues.Count > 0)
			{
				foreach (var value in listValues)
				{
					stringBuilder.AppendFormat(", {0}", value);
				}

				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(0, 2);
				}
			}
			return stringBuilder.ToString();
		}

		public static string FormatStringSQL(string val)
		{
			if (!string.IsNullOrEmpty(val))
			{
				return val.Replace("'", "''");
			}

			return val;
		}
		public static string FormatListDataToListSQL(string[] listValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var value in listValues)
			{
				if (Regex.IsMatch(value, @"^[0-9a-zA-Z:,.+ ]+$"))
					stringBuilder.AppendFormat(", '{0}'", value);
			}

			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 2);
			}

			return stringBuilder.ToString();
		}
		public static string FormatListData(List<int> listValues)
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

		public static string FormatListDataToListSQL(List<string> listValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var value in listValues)
			{
				stringBuilder.AppendFormat(", '{0}'", value);
			}

			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 2);
			}

			return stringBuilder.ToString();
		}
		public static bool CheckFuntion(string FunctionAllow, string FunctionCheck)
		{
			//if (Strings.InStr(1, FunctionAllow, FunctionCheck, CompareMethod.Text) > 0)
			if (FunctionAllow.IndexOf(FunctionCheck, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string ConvertParamSQLStringDirect(string Str)
		{
			if (!string.IsNullOrEmpty(Str))
			{
				return Str.Replace("'", "''");
			}

			return Str;
		}

		//Hàm tạo SID
		public static string TryToGetSID(DateTime date, string seq)
		{
			string SID = "";
			if (seq.IndexOf("-") == 6)
			{
				SID = seq;
			}
			else
			{
				SID = string.Format("{0:ddMMyy}-{1}", date, seq.Trim());
			}

			return SID;

		}

		public static string GetEntryDirectoryName()
		{
			string executingPath = System.Reflection.Assembly.GetEntryAssembly().Location;
			var result = new FileInfo(executingPath).DirectoryName;
			return result;
		}

		public static Color GetColorFromCode(string colorCode)
		{
			try
			{
				if (colorCode.Length >= 6)
				{
					int red = Int32.Parse(colorCode.Substring(0, 2), NumberStyles.HexNumber);
					int green = Int32.Parse(colorCode.Substring(2, 2), NumberStyles.HexNumber);
					int blue = Int32.Parse(colorCode.Substring(4, 2), NumberStyles.HexNumber);

					return Color.FromArgb(red, green, blue);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(string.Format("Không thể convert mã màu {0} !", colorCode), ex);
			}
			return Color.Black;
		}
		/// <summary>
		/// //Thiết lập định dạng thời gian và dấu chấm động
		/// </summary>
		public static void SetFormatSystem()
		{
			var dateTimeInfo = new DateTimeFormatInfo();
			dateTimeInfo.DateSeparator = "-";
			dateTimeInfo.LongDatePattern = "yyyy-MM-dd";
			dateTimeInfo.ShortDatePattern = "yy-MM-dd";
			dateTimeInfo.LongTimePattern = "hh:mm:ss";
			dateTimeInfo.ShortTimePattern = "hh:mm";

			var ci = new CultureInfo("en-us");
			ci.DateTimeFormat = dateTimeInfo;
			ci.NumberFormat.NumberGroupSeparator = ",";//Dấu phẩy phân cách hàng nghìn
			ci.NumberFormat.NumberDecimalSeparator = ".";//Dấu chấm phân cách phần thập phân

			CultureInfo.DefaultThreadCurrentCulture = ci;
			CultureInfo.DefaultThreadCurrentUICulture = ci;

			Thread.CurrentThread.CurrentCulture = ci;
			Thread.CurrentThread.CurrentUICulture = ci;
			//***********************************************************
		}
		/// <summary>
		/// //Đọc số tiền bằng chữ
		/// </summary>
		public static string NumberPriceToTextVN(decimal total)
		{
			try
			{
				string rs = "";
				total = Math.Round(total, 0);
				string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
				string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
				string[] u = { "", "mươi", "trăm", "nghìn", "", "", "triệu", "", "", "tỷ", "", "", "nghìn", "", "", "triệu" };
				string nstr = total.ToString();

				int[] n = new int[nstr.Length];
				int len = n.Length;
				for (int i = 0; i < len; i++)
				{
					n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
				}

				for (int i = len - 1; i >= 0; i--)
				{
					if (i % 3 == 2)// số 0 ở hàng trăm
					{
						if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
					}
					else if (i % 3 == 1) // số ở hàng chục
					{
						if (n[i] == 0)
						{
							if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
							else
							{
								rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
							}
						}
						if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
						{
							rs += " mười"; continue;
						}
					}
					else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
					{
						if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
						{
							if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
							rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
							continue;
						}
						if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
						{
							rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
							rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
							continue;
						}
						if (n[i] == 5) // cách đọc số 5
						{
							if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
							{
								rs += " " + rch[n[i]];// đọc số 
								rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
								continue;
							}
						}
					}

					rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
					rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
				}
				if (rs[rs.Length - 1] != ' ')
					rs += " đồng";
				else
					rs += "đồng";

				if (rs.Length > 2)
				{
					string rs1 = rs.Substring(0, 2);
					rs1 = rs1.ToUpper();
					rs = rs.Substring(2);
					rs = rs1 + rs;
				}
				return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười").Replace(",", "");
			}
			catch
			{
				return "";
			}

		}

		public static DataTable ConvertToDataTable<T>(IList<T> data)
		{
			PropertyDescriptorCollection properties =
			   TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable();
			foreach (PropertyDescriptor prop in properties)
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in properties)
					row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
				table.Rows.Add(row);
			}
			return table;

		}

		public static string Serialize<T>(T serialObj)
		{
			string result = null;
			var serializer = new XmlSerializer(typeof(T));
			using (var memoryStream = new MemoryStream())
			{
				var settings = new XmlWriterSettings();
				settings.Encoding = UTF8Encoding.UTF8;
				settings.Indent = true;
				settings.OmitXmlDeclaration = true;
				using (var xmlTextWriter = XmlWriter.Create(memoryStream, settings))
				{
					serializer.Serialize(xmlTextWriter, serialObj);
				}

				result = Encoding.UTF8.GetString(memoryStream.ToArray());
			}

			return result;
		}

		public static T Deserialize<T>(string xmlContent)
		{
			T result = default(T);

			if (!string.IsNullOrEmpty(xmlContent))
			{
				var serializer = new XmlSerializer(typeof(T));
				using (var readStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent)))
				{
					var reader = new XmlTextReader(readStream);
					result = (T)serializer.Deserialize(reader);
				}
			}
			return result;
		}
		public static T CloneModel<T>(T data)
		{
			return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(data));
		}
		public static string JsonSerialize(object data)
		{
			return JsonSerializer.Serialize(data, new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All) });
		}
		public static T JsonDeserialize<T>(string data)
		{
			return JsonSerializer.Deserialize<T>(data);
		}
		public static string CovnertParameterUrlGet(string _para)
		{
			if (_para.IndexOf("[DauThang]") > -1)
			{
				_para = _para.Replace("[DauThang]", "#");
			}
			return _para;
		}

		/// <summary>
		/// Hàm cắt đi dấu nháy trong chuổi
		/// </summary>
		/// <param name="Str">Chuổi ký tự truyền vào</param>
		/// <returns>Chuổi ký tự trả về</returns>
		public static string ConvertString(string strValue)
		{
			if (!string.IsNullOrEmpty(strValue))
			{
				string strConvert = strValue.Replace("'", "''");
				return strConvert;
			}

			return strValue;
		}
	}

}
