using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using EAS.Data.Adapter;
using EAS.Data.Design;
using System.Web.UI.WebControls;

namespace EAS.Data
{
    [ToolboxItem(true)]
    [Description("����ʵ����UI�ؼ���������")]
    [ProvideProperty("WinMapping", typeof(System.Windows.Forms.Control))]
    [ProvideProperty("WebMapping", typeof(System.Web.UI.Control))]
    [DesignerSerializer(typeof(ControllerCodeDomSerializer), typeof(CodeDomSerializer))]
    public partial class DataUIMapper : Component, IContainer, IExtenderProvider
    {
        private MapperInfoList miList = new MapperInfoList();
        private object dataSource = new object();
        //private bool displayMember =false;
        private System.Type type;

        private IAdapter adapter;
        
        public DataUIMapper()
        {
            InitializeComponent();
        }

        public DataUIMapper(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        /// <summary>
        /// ��������
        /// </summary>
        internal IAdapter Adapter
        {
            get
            {
                return this.adapter;
            }
            set
            {
                this.adapter = value;
            }
        }

        /// <summary>
        /// ���͡�
        /// </summary>
        internal System.Type Type
        {
            get
            {
                return this.type;
            }
        }

        #region WinForm

        [Category("����ӳ��")]
        [Description("��ȡ/�������ݰ󶨵�ӳ���ϵ(WinForm)��")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MergableProperty(true)]
        public MapperInfo GetWinMapping(System.Windows.Forms.Control control)
        {
            if (this.Mappings.ContainsKey(control.Name))
                return this.Mappings[control.Name];

            MapperInfo mi2 = new  MapperInfo(this, control.Name);
            this.Mappings.Add(mi2);
            return mi2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetWinMapping(System.Windows.Forms.Control control, MapperInfo value)
        {
            if (this.Mappings.ContainsKey(control.Name))
            {
                MapperInfo mi = this.Mappings[control.Name];
                mi.ControlProperty = value.ControlProperty;
                mi.DataProperty = value.DataProperty;

                if(mi.ControlProperty.Trim().Length ==0)
                {
                    WinHelper.SetDefaultControlProperty(control, ref mi);
                }

                mi.Format = value.Format;
            }
            else
            {
                value.ControlID = control.Name;
                value.DataUIMapper = this;
                this.Mappings.Add(value);
            }
        }

        #endregion

        #region WebForm

        [Category("����ӳ��")]
        [Description("��ȡ/�������ݰ󶨵�ӳ���ϵ(WebForm)��")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MergableProperty(true)]
        public MapperInfo GetWebMapping(System.Web.UI.Control control)
        {
            if (this.Mappings.ContainsKey(control.ID))
                return this.Mappings[control.ID];

            MapperInfo mi2 = new MapperInfo(this, control.ID);
            this.Mappings.Add(mi2);
            return mi2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetWebMapping(System.Web.UI.Control control, MapperInfo value)
        {
            if (this.Mappings.ContainsKey(control.ID))
            {
                MapperInfo mi = this.Mappings[control.ID];
                mi.ControlProperty = value.ControlProperty;
                mi.DataProperty = value.DataProperty;
                if (mi.ControlProperty.Trim().Length == 0)
                {
                    WebHelper.SetDefaultControlProperty(control, ref mi);
                }
                mi.Format = value.Format;
            }
            else
            {
                value.ControlID = control.ID;
                value.DataUIMapper = this;
                this.Mappings.Add(value);
            }
        }

        #endregion

        //[Category("����ӳ��")]
        //[Description("��ȡ/����һ��ֵָʾ�Ƿ���ʾ�󶨳�Ա��")]
        //[Browsable(true)]
        //[DefaultValue(false)]
        //public bool DisplayMember
        //{
        //    get
        //    {
        //        return this.displayMember;
        //    }
        //    set
        //    {
        //        this.displayMember = value;
        //    }
        //}

        [Category("����ӳ��")]
        [Description("��ȡ/���ð󶨵�����Դ��")]
        [Browsable(true)]
        [DefaultValue((string)null),
        RefreshProperties(RefreshProperties.Repaint),
       AttributeProvider(typeof(IListSource)),
        ]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                this.dataSource = value;

                if (this.dataSource != null)
                {
                    if (this.dataSource is System.Type)
                    {
                        this.type = this.dataSource as System.Type;
                    }
                    else if (this.dataSource is BindingSource)
                    {
                        try
                        {
                        object  var = (this.dataSource as BindingSource).DataSource;
                        if (var is System.Type)
                            this.type = var as System.Type;
                        else
                            this.type = var.GetType();
                        }catch{}
                    }
                    else if (this.dataSource is ObjectDataSource)
                    {
                        try
                        {
                        this.type = System.Type.GetType((this.dataSource as ObjectDataSource).DataObjectTypeName);
                        }catch{}
                    }
                    else
                    {
                        this.type = this.dataSource.GetType();
                    }
                }
            }
        }

        [Category("����ӳ��")]
        [Description("����������UI�İ����ϵ��")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
		[Editor(typeof(MappingsEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MapperInfoList Mappings
        {
            get
            {
                return miList;
            }
            set
            {
                this.miList = value;
            }
        }

        //[Category("����ӳ��")]
        //[Description("��ȡ/���ð󶨵�����Դ���͡�")]
        //[DefaultValue("")]
        //[Browsable(true)]
        //public string DataSourceType
        //{
        //    get
        //    {
        //        return this.dsType;
        //    }
        //    set
        //    {
        //        this.dsType = value;

        //        if (this.dsType != null)
        //        {
        //            try
        //            {
        //                this.type = System.Type.GetType(this.DataSourceType);
        //            }
        //            catch
        //            {
        //                //
        //            }
        //        }
        //    }
        //}

        #region IContainer ��Ա

        public void Remove(IComponent component)
        {
            components.Remove(component);
        }

        public void Add(IComponent component, string name)
        {
            components.Add(component, name);
        }

        public void Add(IComponent component)
        {
            components.Add(component);
        }

        [Browsable(false)]
        public ComponentCollection Components
        {
            get
            {
                return components.Components;
            }
        }

        #endregion

        #region IExtenderProvider ��Ա

        bool IExtenderProvider.CanExtend(object extendee)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (extendee == host.RootComponent)
            {
                return false;
            }
            else if (extendee is System.Windows.Forms.Control)
            {
                return true;
            }
            else if (extendee is System.Web.UI.Control)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// ��������Դ��ֵ��
        /// </summary>
        public void UpdateObject()
        {
            if (this.Adapter != null)
                this.Adapter.UpdateObject(this);
        }

        /// <summary>
        /// ��������Դ��ֵ��
        /// </summary>
        public void UpdateObject(object dataSource)
        {
            this.dataSource = dataSource;
            this.UpdateObject();
        }

        /// <summary>
        /// ���½�����ʾ��
        /// </summary>
        public void UpdateUI()
        {
            if(this.Adapter!=null)
                this.Adapter.UpdateUI(this);
        }

        /// <summary>
        /// ���½�����ʾ��
        /// </summary>
        public void UpdateUI(object dataSource)
        {
            this.dataSource = dataSource;
            this.UpdateUI();
        }        
    }
}
