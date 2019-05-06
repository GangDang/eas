using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using EAS.Explorer.Entities;
using EAS.Data.ORM;

namespace EAS.ActiveXForm.Web
{
    public partial class MainContainer : System.Web.UI.Page
    {
        private string guid = string.Empty;
        private string name = string.Empty;
        private string module = string.Empty;
        private string tag = string.Empty;
        private string param = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.param = base.Request.Params["login"].ToString();
            string[] sv = this.param.Split(new char[] { ',' });

            this.guid = sv[0];
            this.name = sv[1];
            this.module = sv[4];

            if (this.name.Trim().Length  == 0)
            {
                EAS.Explorer.Entities.Module doModule = new EAS.Explorer.Entities.Module();
                doModule.Guid = this.guid;
                doModule.Refresh();
                this.name = doModule.Name;
                this.module = doModule.Assembly+"|"+doModule.Type;
            }

            if (sv.Length == 7)
            {
                this.tag = sv[6];
            }
        }

        /// <summary>
        /// ģ��GUID��
        /// </summary>
        public string Guid
        {
            get
            {
                return this.guid;
            }
        }

        /// <summary>
        /// ģ�����ơ�
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string Param
        {
            get
            {
                return this.param;
            }
        }

        /// <summary>
        /// ģ����Ϣ��
        /// </summary>
        public string Module
        {
            get
            {
                return this.module;
            }
        }

        /// <summary>
        /// ��ǡ�
        /// </summary>
        public string Tag
        {
            get
            {
                return this.tag;
            }
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public string StartParam
        {
            get
            {
                return this.Guid + "|" + this.Module;
            }
        }
    }
}
