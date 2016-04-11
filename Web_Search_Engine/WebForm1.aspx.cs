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
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String conn = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            Database db = new Database(conn);
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(255);
            dict.Add(333, list);

            List<int> list2 = new List<int>();
            list2.Add(789);
            list2.Add(222);
            list2.Add(122);

            dict.Add(222, list2);
            db.obtainDictFromTable(0);
        }

    }
}