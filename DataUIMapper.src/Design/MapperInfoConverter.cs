using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Collections;
using System.ComponentModel;

namespace EAS.Data.Design
{
    /// <summary>
    /// �ؼ�����ת������
    /// </summary>
    internal class ControlPropertyConverter : StringListConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            MapperInfo info = (MapperInfo)context.Instance;

            //if (!info.DataUIMapper.DisplayMember)
            //{
            //    string[] sv = new string[] { string.Empty };
            //    return new StandardValuesCollection(sv);
            //}

            IDesignerHost host = (IDesignerHost)context.GetService(typeof(IDesignerHost));
            IReferenceService svc = (IReferenceService)host.GetService(typeof(IReferenceService));

            if (svc == null)
                return null;

            object ctl = svc.GetReference(info.ControlID);

            if (ctl == null) 
            {
                throw new ArgumentException("��ǰ������δ��������Ϊ '" + info.ControlID + "' �Ŀؼ���");
            }
            else
            {
                return new StandardValuesCollection(MemberHelper.GetControlProperties(ctl));
            }
        }
    }

    /// <summary>
    /// ��������ת������
    /// </summary>
    internal class DataPropertyConverter : StringListConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                MapperInfo info = (MapperInfo)context.Instance;
                return new StandardValuesCollection(MemberHelper.GetDataPropertys(info.DataUIMapper.Type));
            }
            catch
            {
                string[] sv = new string[] { string.Empty };
                return new StandardValuesCollection(sv);
            }
        }
    }
}
