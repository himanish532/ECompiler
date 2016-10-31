using MySql.Data.MySqlClient;
using Oracle.DataAccess.Client;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO; 
using System.Data;
using System.Text;
using System.Web;
using System;
using System.Xml.Serialization;
using System.Xml;

[Serializable]
public class CustomData{
    public DataSet d;
    public string DB_Name;
    public string DB_Username;
    public string DB_Query;
    
    public void Set_Data(DataSet ds,  string name, string username, string query){
        d = ds;
        DB_Name = name;
        DB_Username = username;
        DB_Query = query;
    }
}

public class Main_Functions {
    public String Get_Data(String DB_Type, String DB_URL, String DB_Name, String DB_Username, String DB_Password, String DB_Query) {
        
        string return_data = "aa";
        CustomData cd = new CustomData();
        var c = HttpContext.Current;        
        DataSet SQLDS = new DataSet();   
           
        if(DB_Type.CompareTo("MySQL") == 0) {
            
            string MySQLConStr = "Server="+DB_URL+";Database="+DB_Name+";Uid="+DB_Username+";Pwd="+DB_Password+";";
            MySqlConnection MySQLCon = new MySqlConnection(MySQLConStr);
            MySqlCommand MySQLCMD;
            MySQLCon.Open();
            try{
                MySQLCMD = MySQLCon.CreateCommand();
                MySQLCMD.CommandText = DB_Query;
                MySqlDataAdapter MySQLADAP = new MySqlDataAdapter(MySQLCMD);            
                MySQLADAP.Fill(SQLDS);              
            } catch(Exception e) {
                return e.ToString();
               
            } finally {
                MySQLCon.Close();                
            }
        } else if (DB_Type.CompareTo("Oracle") == 0) {
            
            string OracleConStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST="+DB_URL+")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="+DB_Name+")));User Id="+DB_Username+";Password="+DB_Password+";";
            OracleConnection OracleCon = new OracleConnection(OracleConStr);
            OracleCommand OracleCMD = new OracleCommand(DB_Query, OracleCon); 
            OracleCon.Open();
            try {
                OracleDataAdapter OracleDAP = new OracleDataAdapter(OracleCMD);
                OracleDAP.Fill(SQLDS);
            } catch(Exception e) {
                return e.ToString();               
            } finally {
                OracleCon.Close();                
            } 
        } else if(DB_Type.CompareTo("MS SQL Server") == 0) { 
            string SQLConStr = "Server="+DB_URL+";Database="+DB_Name+";User Id="+DB_Username+";Password="+DB_Password+";";
            SqlConnection SQLCon = new SqlConnection(SQLConStr);
            SqlCommand SQLCMD = new SqlCommand(DB_Query, SQLCon);
            SQLCon.Open();
            try {
                SqlDataAdapter SQLDAP = new SqlDataAdapter(SQLCMD);
                SQLDAP.Fill(SQLDS);
            } catch(Exception e) {
                return e.ToString();
            } finally {
                SQLCon.Close();
            }
        }

        cd.Set_Data(SQLDS, DB_Name, DB_Username, DB_Query);  
        XmlSerializer ser = new XmlSerializer(cd.GetType());
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.IO.StringWriter writer = new System.IO.StringWriter(sb);
        ser.Serialize(writer, cd);           
        return sb.ToString();
    }    

    public CustomData Deserial(String data) {
        CustomData c = new CustomData();
        XmlSerializer ser = new XmlSerializer(c.GetType());
        XmlDocument doc = new XmlDocument();
        doc.LoadXml (data);
        XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
        object obj = ser.Deserialize(reader);
        return (CustomData) obj;
    }
}