using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Web_Search_Engine
{
    public class Database
    {
        private string conString;
        public Database(string con)
        {
            conString = con;
        }

        /*0=parent
        1=children
        2=keywords
        3=content forward index
        4=content inverted index
        5=title forward index
        6=title inverted index
        7=websites
        */
        public void clearTable()
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("cleartable", con))
                {
                    con.Open();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOrDeleteChildrenParent(int mainId, int contentId, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("childrenParent", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@mainId", SqlDbType.Int).Value = mainId;
                    command.Parameters.Add("@contentId", SqlDbType.Int).Value = contentId;
                    command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOrDeleteForwardIndex(int pageId, string word, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("forwardIndex", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@mainId", SqlDbType.Int).Value = pageId;
                    command.Parameters.Add("@content", SqlDbType.NVarChar).Value = word;
                    command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOrDeleteInvertedIndex(int wordId, int pageId, int position, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("invertedIndex", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@mainId", SqlDbType.Int).Value = wordId;
                    command.Parameters.Add("@pageId", SqlDbType.Int).Value = pageId;
                    command.Parameters.Add("@position", SqlDbType.Int).Value = position;
                    command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOrDeleteKeyword(int wordId, string word)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("keywordPro", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@wordId", SqlDbType.Int).Value = wordId;
                    command.Parameters.Add("@word", SqlDbType.NVarChar).Value = word;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOrDeletePage(int id, string url, string lastModified, int contentLength, string title)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("pagePro", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@url", SqlDbType.NVarChar).Value = url;
                    command.Parameters.Add("@lastModified", SqlDbType.NVarChar).Value = lastModified;
                    command.Parameters.Add("@contentLength", SqlDbType.Int).Value = contentLength;
                    command.Parameters.Add("@title", SqlDbType.NVarChar).Value = title;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void updatePage(int id, string url, string lastModified, int contentLength, string title)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("page_procedure", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@url", SqlDbType.NVarChar).Value = url;
                    command.Parameters.Add("@lastModified", SqlDbType.NVarChar).Value = lastModified;
                    command.Parameters.Add("@contentLength", SqlDbType.Int).Value = contentLength;
                    command.Parameters.Add("@title", SqlDbType.NVarChar).Value = title;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Dictionary<int,List<int>> getChildrenOrParent(int mainId, int flag)
        {
            string conn = conString;
            string cmd = "";
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            if(flag==0)
            {
                cmd = "SELECT * FROM _parent WHERE parentId= " + mainId;
            }
            else if(flag==1)
            {
                cmd = "SELECT * FROM _children WHERE childId= " + mainId;
            }
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int autoid = reader.GetInt32(0);
                            int fromId = reader.GetInt32(1);
                            int getId = reader.GetInt32(2);
                            //foreach (string str in array)
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                            //    list.Add(Convert.ToInt32(str));
                            //}
                            List<int> list = new List<int>();
                            list.Add(fromId);
                            list.Add(getId);
                            dict.Add(autoid, list);
                        }
                    }
                }
            }
            return dict;
        }

        public List<Tuple<int,int,string>> getForwardIndex(int mainId,int flag)
        {
            string conn = conString;
            string cmd = "";
            List<Tuple<int, int, string>> list = new List<Tuple<int, int, string>>();
            if(flag==3)
            {
                cmd = "SELECT * FROM _content_forward_index WHERE pageId= " + mainId;
            }
            else if(flag==5)
            {
                cmd = "SELECT * FROM _title_forward_index WHERE pageId= " + mainId;
            }
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int autoid = reader.GetInt32(0);
                            int pageId = reader.GetInt32(1);
                            string word = reader.GetString(2);
                            Tuple<int, int, string> tuple = new Tuple<int, int, string>(autoid, pageId, word);
                            //foreach (string str in array)
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                            //    list.Add(Convert.ToInt32(str));
                            //}
                            list.Add(tuple);
                        }
                    }
                }
            }

            return list;
        }

        public List<Tuple<int,int,int,int>> getInvertedIndex(int mainId,int flag)
        {
            string conn = conString;
            string cmd = "";
            List<Tuple<int, int, int, int>> list = new List<Tuple<int, int, int, int>>();
            if (flag == 4)
            {
                cmd = "SELECT * FROM _content_inverted_index WHERE wordId= " + mainId;
            }
            else if(flag==6)
            {
                cmd = "SELECT * FROM _title_inverted_index WHERE wordId= " + mainId;
            }
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            int autoId = reader.GetInt32(0);
                            int wordId = reader.GetInt32(1);
                            int pageId = reader.GetInt32(2);
                            int position = reader.GetInt32(3);
                            Tuple<int, int, int, int> tuple = new Tuple<int, int, int, int>(autoId, wordId, pageId, position);
                            list.Add(tuple);
                        }
                    }
                }
            }
            return list;
        }

        public string getKeyword(int wordId)
        {
            string conn = conString;
            string cmd = "SELECT * FROM keyword WHERE wordId= " + wordId;
            string keyword = "";
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand)
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            keyword = reader.GetString(1);
                        }
                    }
                }
            }
            return keyword;
        }

        public Tuple<int,string,string,int,string> getPage(int id)
        {
            string conn = conString;
            string cmd = "SELECT * FROM page WHERE Id=" + id;
            Tuple<int, string, string, int, string> tup = null;
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            tup = new Tuple<int, string, string, int, string>(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4));
                        }
                    }
                }
            }
            return tup;
        }

        //
        public void insertDictToTable(Dictionary<int,string> dict, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                foreach (KeyValuePair<int, string> pair in dict)
                {
                    using (SqlCommand command = new SqlCommand("Procedure", con))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@param1", SqlDbType.Int).Value = pair.Key;
                        command.Parameters.Add("@param2", SqlDbType.NVarChar).Value = pair.Value;
                        command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                        con.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public void insertDictToPage(Dictionary<int,Page> dict, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                foreach (KeyValuePair<int, Page> pair in dict)
                {
                    using (SqlCommand command = new SqlCommand("page_procedure", con))
                    {
                        //Page pa = (Page)pair.Value;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = pair.Key;
                        command.Parameters.Add("@contentLength", SqlDbType.Int).Value = pair.Value.ContentLength;
                        command.Parameters.Add("@url", SqlDbType.NVarChar).Value = pair.Value.Url;
                        command.Parameters.Add("@lastModified", SqlDbType.NVarChar).Value = pair.Value.LastModified;
                        command.Parameters.Add("@title", SqlDbType.NVarChar).Value = pair.Value.Title;
                        con.Open();
                        command.ExecuteNonQuery();
                    }

                }
            }

        }

        public void insertDictToTable(Dictionary<int,List<string>> dict, int flag)
        {
            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                foreach (KeyValuePair<int, List<string>> pair in dict)
                {
                    string sepstring = string.Join(" ", pair.Value);

                    using (SqlCommand command = new SqlCommand("Procedure", con))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@param1", SqlDbType.Int).Value = pair.Key;
                        command.Parameters.Add("@param2", SqlDbType.NVarChar).Value = sepstring;
                        command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                        con.Open();
                        command.ExecuteNonQuery();
                    }

                }
            }

        }

        public void insertDictToTable(Dictionary<int,List<int>> dict, int flag)
        {
            //string conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            //Response.Write("<Script language='JavaScript'>alert('" + conn + "');</Script>");
            /*
            string commasepstring = string.Join(",", list);
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand("Procedure", con))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@param1", SqlDbType.Int).Value = index;
                    command.Parameters.Add("@param2", SqlDbType.NVarChar).Value = commasepstring;
                    command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            }
            */


            string conn = conString;
            using (SqlConnection con = new SqlConnection(conn))
            {
                foreach (KeyValuePair<int, List<int>> pair in dict)
                {
                    string sepstring = string.Join(" ", pair.Value);

                    using (SqlCommand command = new SqlCommand("Procedure", con))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@param1", SqlDbType.Int).Value = pair.Key;
                        command.Parameters.Add("@param2", SqlDbType.NVarChar).Value = sepstring;
                        command.Parameters.Add("@param3", SqlDbType.Int).Value = flag;
                        con.Open();
                        command.ExecuteNonQuery();
                    }

                }
            }

            //con.Open();
        }

        public Dictionary<int, List<int>> obtainDictFromTable(int flag)
        {
            //string conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            string conn = conString;
            string cmd = "";
            if (flag == 0)
            {
                cmd = "SELECT * FROM parent ";
            }
            else
            {
                cmd = "SELECT * FROM children ";
            }
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string result = reader.GetString(1);
                            string[] array = result.Split(' ');
                            List<int> list = new List<int>();
                            foreach (string str in array)
                            {
                                //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                                if(str.Trim().Length>0)
                                {
                                    list.Add(Convert.ToInt32(str));
                                }
                            }
                            dict.Add(id, list);
                        }
                    }
                }
            }
            return dict;
        }

        /*0=parent
        1=children
        2=keywords
        3=content forward index
        4=content inverted index
        5=title forward index
        6=title inverted index
        */
        public Dictionary<int,string> obtainDictFromTableString(int flag)
        {
            //string conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            string conn = conString;
            string cmd = "";
            switch (flag)
            {
                case 2: cmd = "SELECT * FROM keyword"; break;
            }
            Dictionary<int, string> dict = new Dictionary<int, string>();
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string result = reader.GetString(1);
                            //foreach (string str in array)
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                            //    list.Add(Convert.ToInt32(str));
                            //}
                            dict.Add(id, result);
                        }
                    }
                }
            }
            return dict;
        }
        public Dictionary<int,List<string>> obtainDictFromTableListString(int flag)
        {
            //string conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            string conn = conString;
            string cmd = "";
            switch (flag)
            {
                case 3: cmd = "SELECT * FROM content_forward_index"; break;
                case 4: cmd = "SELECT * FROM content_inverted_index"; break;
                case 5: cmd = "SELECT * FROM title_forward_index"; break;
                case 6: cmd = "SELECT * FROM title_inverted_index"; break;
            }
            Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string result = reader.GetString(1);
                            List<string> list = result.Split(' ').ToList<string>();
                            //foreach (string str in array)
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                            //    list.Add(Convert.ToInt32(str));
                            //}
                            dict.Add(id, list);
                        }
                    }
                }
            }
            return dict;
        }

        public Dictionary<int,Page> obtainDictFromPage()
        {
            //string conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            string conn = conString;
            string cmd = "SELECT * FROM page";
            Dictionary<int, Page> dict = new Dictionary<int, Page>();
            using (SqlConnection con = new SqlConnection(conn))
            {
                using (SqlCommand command = new SqlCommand(cmd, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string url = reader.GetString(1);
                            string lastModified = reader.GetString(2);
                            int contentLength = reader.GetInt32(3);
                            string title = reader.GetString(4);
                            Page pg = new Page(id, url, lastModified, contentLength, title);
                            //foreach (string str in array)
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('" + str + "');</Script>");
                            //    list.Add(Convert.ToInt32(str));
                            //}
                            dict.Add(id, pg);
                        }
                    }
                }
            }
            return dict;
        }






    }
}