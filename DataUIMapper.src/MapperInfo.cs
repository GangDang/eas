using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using EAS.Data.Design;

namespace EAS.Data
{
    public class MapperInfoList : List<MapperInfo>
    {
        public new  void Add(MapperInfo item)
        {
            if (Exists(item.ControlID))
                throw new System.Exception("�Ѿ����ڿؼ�:" + item.ControlID + "������ӳ��");

            base.Add(item);
        }

        public bool Exists(string controlID)
        {
            foreach (MapperInfo item in this)
            {
                if (item.ControlID == controlID)
                    return true;
            }

            return false;
        }

        public bool ContainsKey(string controlID)
        {
            return this.IndexOfKey(controlID) > -1;
        }

        public int IndexOfKey(string controlID)
        {
            int index = -1;

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].ControlID == controlID)
                    return i;
            }

            return index;
        }

        public MapperInfo this[string Key]
        {
            get
            {
                int index = this.IndexOfKey(Key);

                if (index > -1)
                    return this[index];
                else
                    return null;
            }
            set
            {
                int index = this.IndexOfKey(Key);

                if (index > -1)
                    this[index] = value;
                else
                    this.Add(value);
            }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapperInfo
    {
        DataUIMapper mapper;

        string ctrlid = String.Empty;
        string dataproperty = String.Empty;
        string controlproperty = String.Empty;
        Format format = Format.None;

        public override string ToString()
        {
            return dataproperty + " -> " + ctrlid + "." + controlproperty;
        }

        public MapperInfo()
        {
        }

        public MapperInfo(DataUIMapper mapper, string controlId)
        {
            this.mapper = mapper;
            this.ctrlid = controlId;
        }

        public MapperInfo(string controlId, string controlProperty, string dataProperty)
        {
            this.ctrlid = controlId;
            this.controlproperty = controlProperty;
            this.dataproperty = dataProperty;
        }

        public MapperInfo(string controlId, string controlProperty, string dataProperty,int format)
        {
            this.ctrlid = controlId;
            this.controlproperty = controlProperty;
            this.dataproperty = dataProperty;
            this.format = (Format)format;
        }

        [Browsable(false)]
        internal DataUIMapper DataUIMapper
        {
            get
            {
                return mapper;
            }
            set
            {
                mapper = value;
            }
        }

        [DefaultValue("")]
        [Browsable(false)]
        [Description("�ؼ�����/ID")]
        public string ControlID
        {
            get
            {
                return ctrlid;
            }
            set
            {
                ctrlid = value;
            }
        }
                
        [DefaultValue("")]
        [Description("�ؼ�����")]
        [TypeConverter(typeof(ControlPropertyConverter))]
        public string ControlProperty
        {
            get
            {
                return controlproperty;
            }
            set
            {
                controlproperty = value;
            }
        }

        [DefaultValue("")]
        [Description("��������")]
        [TypeConverter(typeof(DataPropertyConverter))]
        public string DataProperty
        {
            get
            {
                return dataproperty;
            }
            set
            {
                dataproperty = value;
            }
        }

        [Description("��ʾ��ʽ")]
        [DefaultValue(Format.None)]
        public Format Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
            }
        }
    }

    /// <summary>
    /// ���ָ�ʽ��
    /// </summary>
    public enum Format
    {
        [Description("�޸�ʽ")]
        None,

        [Description("����")]
        Date,

        [Description("ʱ��")]
        Time,

        [Description("���ں�ʱ��")]
        DateAndTime,

        [Description("2λС��")]
        F2,

        [Description("4λС��")]
        F4,

        [Description("6λС��")]
        F6,

        [Description("2λС��/����")]
        MF2,

        [Description("4λС��/����")]
        MF4,

        [Description("6λС��/����")]
        MF6,
    }
}
