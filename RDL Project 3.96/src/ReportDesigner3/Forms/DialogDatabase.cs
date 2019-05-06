using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using fyiReporting.RDL;
using System.Xml;
using System.IO;
using Microsoft.Data.ConnectionUI;
using EAS.Configuration;

namespace fyiReporting.RdlDesign
{
    partial class DialogDatabase : Form
    {
        string ConnectionString = null;

        private List<SqlColumn> _ColumnList = null;
        private string _TempFileName = null;
        private string _ResultReport = "nothing";

        private readonly string _Schema2003 = CacheHelper.Instance.Schema2003;
        private readonly string _Schema2005 = CacheHelper.Instance.Schema2005;

        private string _TemplateChart = CacheHelper.Instance.TemplateChart;
        private string _TemplateMatrix = CacheHelper.Instance.TemplateMatrix;
        private string _TemplateTable = CacheHelper.Instance.TemplateTable;
        private string _TemplateList = CacheHelper.Instance.TemplateList;

        public DialogDatabase()
        {
            InitializeComponent();

            try
            {
                string provider = Config.GetValue("DefaultProvider");

                if (string.Compare(provider, "MSSqlServer", true) == 0)
                {
                    this.cbConnectionTypes.SelectedIndex = 0;
                }
                else if (string.Compare(provider, "OleDBSupported", true) == 0)
                {
                    this.cbConnectionTypes.SelectedIndex = 1;
                }
                else if (string.Compare(provider, "Oracle", true) == 0)
                {
                    this.cbConnectionTypes.SelectedIndex = 2;
                }
                else if (string.Compare(provider, "ODBC", true) == 0)
                {
                    this.cbConnectionTypes.SelectedIndex = 3;
                }

                this.tbConnection.Text = EAS.Configuration.Config.GetValue("DBConnectString");

                this.cbOrientation.SelectedIndex = 0;

                this.cbxPages.Items.Clear();

                foreach (PrintPage page in CacheHelper.Instance.Pages)
                {
                    this.cbxPages.Items.Add(page.Name);
                }

                this.cbxPages.SelectedIndexChanged += new EventHandler(cbxPages_SelectedIndexChanged);
                this.cbxPages.SelectedIndex = 4;
            }
            catch { }
        }

        void cbxPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrintPage page = CacheHelper.Instance.Pages[this.cbxPages.SelectedIndex];

            if (this.cbxPages.SelectedIndex != this.cbxPages.Items.Count - 1)
            {
                this.tbWidth.Text = page.WidthString;
                this.tbHeight.Text = page.HeightString;

                this.tbHeight.Enabled = this.tbWidth.Enabled = false;
            }
            else
            {
                this.tbHeight.Enabled = this.tbWidth.Enabled = true;
                this.tbWidth.Text = string.Empty;
                this.tbHeight.Text = string.Empty;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_TempFileName != null)
                File.Delete(_TempFileName);

            string provider = "OleDBSupported";

            if (this.cbConnectionTypes.SelectedIndex == 0)
            {
                provider = "OleDBSupported";
            }
            else if (this.cbConnectionTypes.SelectedIndex == 1)
            {
                provider = "MSSqlServer";
            }
            else if (this.cbConnectionTypes.SelectedIndex == 2)
            {
                provider = "Oracle";
            }
            else if (this.cbConnectionTypes.SelectedIndex == 3)
            {
                provider = "ODBC";
            }

            try
            {
                EAS.Configuration.Config.SetValue("DefaultProvider", provider);
                EAS.Configuration.Config.SetValue("DBConnectString", this.tbConnection.Text);
                EAS.Configuration.Config.Save(Application.ExecutablePath + ".config");
            }
            catch { }

            base.OnClosed(e);
        }

        public string ResultReport
        {
            get { return _ResultReport; }
        }

        public string QueyText
        {
            get
            {
                return this.tbSQL.Text;
            }
            set
            {
                this.tbSQL.Text = value;
            }
        }

        #region �������

        // Fill out tvTablesColumns
        private void DoSqlSchema()
        {
            // TODO be more efficient and remember schema info;
            //   need to mark changes to connections
            if (tvTablesColumns.Nodes.Count > 0)
                return;

            // suppress redraw until tree view is complete
            tvTablesColumns.BeginUpdate();

            // Get the schema information
            List<SqlSchemaInfo> si = DesignerUtility.GetSchemaInfo(GetDataProvider(), GetDataConnection());
            TreeNode ndRoot = new TreeNode("Tables");
            tvTablesColumns.Nodes.Add(ndRoot);
            bool bView = false;
            foreach (SqlSchemaInfo ssi in si)
            {
                if (!bView && ssi.Type == "VIEW")
                {	// Switch over to views
                    ndRoot = new TreeNode("Views");
                    tvTablesColumns.Nodes.Add(ndRoot);
                    bView = true;
                }

                // Add the node to the tree
                TreeNode aRoot = new TreeNode("[" + ssi.Schema + "].[" + ssi.Name+"]");

                //TreeNode aRoot = new TreeNode(ssi.Schema + "." + ssi.Name);
                //TreeNode aRoot = new TreeNode( ssi.Name);

                ndRoot.Nodes.Add(aRoot);
                aRoot.Nodes.Add("");
            }

            // Now do parameters
            if (lbParameters.Items.Count > 0)
            {
                ndRoot = new TreeNode("Parameters");
                tvTablesColumns.Nodes.Add(ndRoot);
                foreach (ReportParm rp in lbParameters.Items)
                {
                    string paramName;

                    // force the name to start with @
                    if (rp.Name[0] == '@')
                        paramName = rp.Name;
                    else
                        paramName = "@" + rp.Name;

                    // Add the node to the tree
                    TreeNode aRoot = new TreeNode(paramName);
                    ndRoot.Nodes.Add(aRoot);
                }
            }

            tvTablesColumns.EndUpdate();
        }

        private void DoGrouping()
        {
            if (cbColumnList.Items.Count > 0)		// We already have the columns?
                return;

            if (_ColumnList == null)
                _ColumnList = DesignerUtility.GetSqlColumns(GetDataProvider(), GetDataConnection(), tbSQL.Text, this.lbParameters.Items);

            foreach (SqlColumn sq in _ColumnList)
            {
                cbColumnList.Items.Add(sq);
                clbSubtotal.Items.Add(sq);
            }

            SqlColumn sqc = new SqlColumn();
            sqc.Name = "";
            cbColumnList.Items.Add(sqc);
            return;
        }

        private bool DoReportSyntax()
        {
            string template;

            if (rbList.Checked)
                template = _TemplateList;
            else if (rbTable.Checked)
                template = _TemplateTable;
            else if (rbMatrix.Checked)
                template = _TemplateMatrix;
            else if (rbChart.Checked)
                template = _TemplateChart;
            else
                template = _TemplateTable;	// default to table- should never reach

            if (_ColumnList == null)
                _ColumnList = DesignerUtility.GetSqlColumns(GetDataProvider(), GetDataConnection(), tbSQL.Text, this.lbParameters.Items);

            if (_ColumnList.Count == 0)		// can only happen by an error
                return false;

            string[] parts = template.Split('|');
            StringBuilder sb = new StringBuilder(template.Length);
            decimal left = 0m;
            decimal width;
            decimal bodyHeight = 0;
            string name;
            int skip = 0;					// skip is used to allow nesting of ifdef 
            string args;
            string canGrow;
            string align;
            // handle the group by column
            string gbcolumn;
            if (this.cbColumnList.Text.Length > 0)
                gbcolumn = GetFieldName(this.cbColumnList.Text);
            else
                gbcolumn = null;

            CultureInfo cinfo = new CultureInfo("", false);

            foreach (string p in parts)
            {
                // Handle conditional special
                if (p.Substring(0, 5) == "ifdef")
                {
                    args = p.Substring(6);
                    switch (args)
                    {
                        case "reportname":
                            if (tbReportName.Text.Length == 0)
                                skip++;
                            break;
                        case "description":
                            if (tbReportDescription.Text.Length == 0)
                                skip++;
                            break;
                        case "author":
                            if (tbReportAuthor.Text.Length == 0)
                                skip++;
                            break;
                        case "grouping":
                            if (gbcolumn == null)
                                skip++;
                            break;
                        case "footers":
                            if (!this.ckbGrandTotal.Checked)
                                skip++;
                            else if (this.clbSubtotal.CheckedItems.Count <= 0)
                                skip++;
                            break;
                        default:
                            throw new Exception(String.Format("Unknown ifdef element {0} specified in template.", args));
                    }
                    continue;
                }

                // if skipping lines (due to ifdef) then go to next endif
                if (skip > 0 && p != "endif")
                    continue;

                switch (p)
                {
                    case "endif":
                        if (skip > 0)
                            skip--;
                        break;
                    case "schema":
                        if (this.rbSchema2003.Checked)
                            sb.Append(_Schema2003);
                        else if (this.rbSchema2005.Checked)
                            sb.Append(_Schema2005);
                        break;
                    case "reportname":
                        sb.Append(tbReportName.Text.Replace('\'', '_'));
                        break;
                    case "reportnameasis":
                        sb.Append(tbReportName.Text);
                        break;
                    case "description":
                        sb.Append(tbReportDescription.Text);
                        break;
                    case "author":
                        sb.Append(tbReportAuthor.Text);
                        break;
                    case "connectionproperties":

                        //if (this.cbConnectionTypes.Text == SHARED_CONNECTION)
                        //{
                        //    string file = this.tbConnection.Text;
                        //    file = Path.GetFileNameWithoutExtension(file);
                        //    sb.AppendFormat("<DataSourceReference>{0}</DataSourceReference>", file);
                        //}
                        //else
                        //    sb.AppendFormat("<ConnectionProperties><DataProvider>{0}</DataProvider><ConnectString>{1}</ConnectString></ConnectionProperties>",
                        //        GetDataProvider(), GetDataConnection());

                        sb.AppendFormat("<ConnectionProperties><DataProvider>{0}</DataProvider><ConnectString>{1}</ConnectString></ConnectionProperties>",
                                GetDataProvider(), GetDataConnection());
                        break;
                    case "dataprovider":
                        sb.Append(GetDataProvider());
                        break;
                    case "connectstring":
                        sb.Append(tbConnection.Text);
                        break;
                    case "columncount":
                        sb.Append(_ColumnList.Count);
                        break;
                    case "orientation":

                        if ((this.cbOrientation.SelectedIndex == 0) & (this.cbxPages.SelectedIndex < this.cbxPages.Items.Count - 1))
                        {
                            sb.Append("<PageHeight>" + this.tbHeight.Text + "</PageHeight><PageWidth>" + this.tbWidth.Text + "</PageWidth>");
                        }
                        else
                        {
                            sb.Append("<PageHeight>" + this.tbWidth.Text + "</PageHeight><PageWidth>" + this.tbHeight.Text + "</PageWidth>");
                        }

                        break;

                    case "groupbycolumn":
                        sb.Append(gbcolumn);
                        break;
                    case "reportparameters":
                        DoReportSyntaxParameters(cinfo, sb);
                        break;
                    case "queryparameters":
                        DoReportSyntaxQParameters(cinfo, sb, tbSQL.Text);
                        break;
                    case "sqltext":
                        sb.Append(tbSQL.Text.Replace("<", "&lt;"));
                        break;
                    case "sqlfields":
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = GetFieldName(sq.Name);
                            string type = sq.DataType.FullName;
                            if (this.rbSchemaNo.Checked)
                                sb.AppendFormat(cinfo, "<Field Name='{0}'><DataField>{1}</DataField><TypeName>{2}</TypeName></Field>", name, sq.Name, type);
                            else
                                sb.AppendFormat(cinfo, "<Field Name='{0}'><DataField>{1}</DataField><rd:TypeName>{2}</rd:TypeName></Field>", name, sq.Name, type);
                        }
                        break;
                    case "listheaders":
                        left = .0m;
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = sq.Name;
                            width = name.Length / 8m;
                            if (width < 1)
                                width = 1;
                            sb.AppendFormat(cinfo, @"
		<Textbox><Top>.3in</Top><Left>{0}in</Left><Width>{1}in</Width><Height>.2in</Height><Value>{2}</Value>
			<Style><FontWeight>Bold</FontWeight><BorderStyle><Bottom>Solid</Bottom></BorderStyle>
				<BorderWidth><Bottom>3pt</Bottom></BorderWidth></Style>
		</Textbox>",
                                left,
                                width,
                                name);
                            left += width;
                        }
                        break;
                    case "listvalues":
                        left = .0m;
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = GetFieldName(sq.Name);
                            DoAlignAndCanGrow(sq.DataType, out canGrow, out align);
                            width = name.Length / 8m;
                            if (width < 1)
                                width = 1;
                            sb.AppendFormat(cinfo, @"
		<Textbox Name='{2}'><Top>.1in</Top><Left>{0}in</Left><Width>{1}in</Width><Height>.25in</Height><Value>=Fields!{2}.Value</Value><CanGrow>{3}</CanGrow><Style>{4}</Style></Textbox>",
                                left, width, name, canGrow, align);
                            left += width;
                        }
                        bodyHeight = .4m;
                        break;
                    case "listwidth":		// in template list width must follow something that sets left
                        sb.AppendFormat(cinfo, "{0}in", left);
                        break;
                    case "tableheaders":
                        // the group by column is always the first one in the table
                        if (gbcolumn != null)
                        {
                            bodyHeight += 12m;
                            sb.AppendFormat(cinfo, @"
							<TableCell>
								<ReportItems><Textbox><Value>{0}</Value><Style><TextAlign>Center</TextAlign><BorderStyle><Default>Solid</Default></BorderStyle><FontWeight>Bold</FontWeight></Style></Textbox></ReportItems>
							</TableCell>",
                                this.cbColumnList.Text);
                        }
                        bodyHeight += 12m;
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = sq.Name;
                            if (name == this.cbColumnList.Text)
                                continue;
                            sb.AppendFormat(cinfo, @"
							<TableCell>
								<ReportItems><Textbox><Value>{0}</Value><Style><TextAlign>Center</TextAlign><BorderStyle><Default>Solid</Default></BorderStyle><FontWeight>Bold</FontWeight></Style></Textbox></ReportItems>
							</TableCell>",
                                name);
                        }
                        break;
                    case "tablecolumns":
                        if (gbcolumn != null)
                        {
                            bodyHeight += 12m;
                            width = gbcolumn.Length / 8m;		// TODO should really use data value
                            if (width < 1)
                                width = 1;
                            sb.AppendFormat(cinfo, @"<TableColumn><Width>{0}in</Width></TableColumn>", width);
                        }
                        bodyHeight += 12m;
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = GetFieldName(sq.Name);
                            if (name == gbcolumn)
                                continue;
                            width = name.Length / 8m;		// TODO should really use data value
                            if (width < 1)
                                width = 1;
                            sb.AppendFormat(cinfo, @"<TableColumn><Width>{0}in</Width></TableColumn>", width);
                        }
                        break;
                    case "tablevalues":
                        bodyHeight += 12m;
                        if (gbcolumn != null)
                        {
                            sb.Append(@"<TableCell>
								<ReportItems><Textbox><Value></Value><Style><BorderStyle><Default>None</Default><Left>Solid</Left></BorderStyle></Style></Textbox></ReportItems>
							</TableCell>");
                        }
                        foreach (SqlColumn sq in _ColumnList)
                        {
                            name = GetFieldName(sq.Name);
                            if (name == gbcolumn)
                                continue;
                            DoAlignAndCanGrow(sq.DataType, out canGrow, out align);
                            sb.AppendFormat(cinfo, @"
							<TableCell>
								<ReportItems><Textbox Name='{0}'><Value>=Fields!{0}.Value</Value><CanGrow>{1}</CanGrow><Style><BorderStyle><Default>Solid</Default></BorderStyle>{2}</Style></Textbox></ReportItems>
							</TableCell>",
                                name, canGrow, align);
                        }
                        break;
                    case "gtablefooters":
                    case "tablefooters":
                        bodyHeight += 12m;
                        canGrow = "false";
                        align = "";
                        string nameprefix = p == "gtablefooters" ? "gf" : "tf";
                        if (gbcolumn != null)	// handle group by column first
                        {
                            int i = clbSubtotal.FindStringExact(this.cbColumnList.Text);
                            SqlColumn sq = i < 0 ? null : (SqlColumn)clbSubtotal.Items[i];
                            if (i >= 0 && clbSubtotal.GetItemChecked(i))
                            {
                                string funct = DesignerUtility.IsNumeric(sq.DataType) ? "Sum" : "Count";

                                DoAlignAndCanGrow(((object)0).GetType(), out canGrow, out align);
                                sb.AppendFormat(cinfo, @"
							<TableCell>
								<ReportItems><Textbox Name='{4}_{0}'><Value>={1}(Fields!{0}.Value)</Value><CanGrow>{2}</CanGrow><Style><BorderStyle><Default>Solid</Default></BorderStyle>{3}</Style></Textbox></ReportItems>
							</TableCell>",
                                    gbcolumn, funct, canGrow, align, nameprefix);
                            }
                            else
                            {
                                sb.AppendFormat(cinfo, "<TableCell><ReportItems><Textbox><Value></Value><Style><BorderStyle><Default>Solid</Default></BorderStyle></Style></Textbox></ReportItems></TableCell>");
                            }
                        }
                        for (int i = 0; i < this.clbSubtotal.Items.Count; i++)
                        {
                            SqlColumn sq = (SqlColumn)clbSubtotal.Items[i];
                            name = GetFieldName(sq.Name);
                            if (name == gbcolumn)
                                continue;
                            if (clbSubtotal.GetItemChecked(i))
                            {
                                string funct = DesignerUtility.IsNumeric(sq.DataType) ? "Sum" : "Count";

                                DoAlignAndCanGrow(((object)0).GetType(), out canGrow, out align);
                                sb.AppendFormat(cinfo, @"
							<TableCell>
								<ReportItems><Textbox Name='{4}_{0}'><Value>={1}(Fields!{0}.Value)</Value><CanGrow>{2}</CanGrow><Style><BorderStyle><Default>Solid</Default></BorderStyle>{3}</Style></Textbox></ReportItems>
							</TableCell>",
                                    name, funct, canGrow, align, nameprefix);
                            }
                            else
                            {
                                sb.AppendFormat(cinfo, "<TableCell><ReportItems><Textbox><Value></Value><Style><BorderStyle><Default>Solid</Default></BorderStyle></Style></Textbox></ReportItems></TableCell>");
                            }
                        }
                        break;
                    case "bodyheight":	// Note: this must follow the table definition
                        sb.AppendFormat(cinfo, "{0}pt", bodyHeight);
                        break;
                    default:
                        sb.Append(p);
                        break;
                }
            }

            try
            {
                tbReportSyntax.Text = DesignerUtility.FormatXml(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Internal Error");
                tbReportSyntax.Text = sb.ToString();
            }
            return true;
        }

        private string GetFieldName(string sqlName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in sqlName)
            {
                if (Char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else
                    sb.Append('_');
            }
            return sb.ToString();
        }

        private void DoAlignAndCanGrow(Type t, out string canGrow, out string align)
        {
            string st = t.ToString();
            switch (st)
            {
                case "System.String":
                    canGrow = "true";
                    align = "<PaddingLeft>2 pt</PaddingLeft>";
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    canGrow = "false";
                    align = "<PaddingRight>2 pt</PaddingRight><TextAlign>right</TextAlign>";
                    break;
                default:
                    canGrow = "false";
                    align = "<PaddingLeft>2 pt</PaddingLeft>";
                    break;
            }
            return;
        }


        private void DoReportSyntaxParameters(CultureInfo cinfo, StringBuilder sb)
        {
            if (this.lbParameters.Items.Count <= 0)
                return;

            sb.Append("<ReportParameters>");
            foreach (ReportParm rp in lbParameters.Items)
            {
                sb.AppendFormat(cinfo, "<ReportParameter Name=\"{0}\">", rp.Name);
                sb.AppendFormat(cinfo, "<DataType>{0}</DataType>", rp.DataType);
                sb.AppendFormat(cinfo, "<Nullable>{0}</Nullable>", rp.AllowNull.ToString());
                if (rp.DefaultValue != null && rp.DefaultValue.Count > 0)
                {
                    sb.AppendFormat(cinfo, "<DefaultValue><Values>");
                    foreach (string dv in rp.DefaultValue)
                    {
                        sb.AppendFormat(cinfo, "<Value>{0}</Value>", XmlUtil.XmlAnsi(dv));
                    }
                    sb.AppendFormat(cinfo, "</Values></DefaultValue>");
                }
                sb.AppendFormat(cinfo, "<AllowBlank>{0}</AllowBlank>", rp.AllowBlank);
                if (rp.Prompt != null && rp.Prompt.Length > 0)
                    sb.AppendFormat(cinfo, "<Prompt>{0}</Prompt>", rp.Prompt);
                if (rp.ValidValues != null && rp.ValidValues.Count > 0)
                {
                    sb.Append("<ValidValues><ParameterValues>");
                    foreach (ParameterValueItem pvi in rp.ValidValues)
                    {
                        sb.Append("<ParameterValue>");
                        sb.AppendFormat(cinfo, "<Value>{0}</Value>", XmlUtil.XmlAnsi(pvi.Value));
                        if (pvi.Label != null)
                            sb.AppendFormat(cinfo, "<Label>{0}</Label>", XmlUtil.XmlAnsi(pvi.Label));
                        sb.Append("</ParameterValue>");
                    }
                    sb.Append("</ParameterValues></ValidValues>");
                }
                sb.Append("</ReportParameter>");
            }
            sb.Append("</ReportParameters>");
        }

        private void DoReportSyntaxQParameters(CultureInfo cinfo, StringBuilder sb, string sql)
        {
            if (this.lbParameters.Items.Count <= 0)
                return;

            bool bFirst = true;
            foreach (ReportParm rp in lbParameters.Items)
            {
                // force the name to start with @
                string paramName;
                if (rp.Name[0] == '@')
                    paramName = rp.Name;
                else
                    paramName = "@" + rp.Name;

                // Only create a query parameter if parameter is used in the query
                if (sql.IndexOf(paramName) >= 0)
                {
                    if (bFirst)
                    {	// Only put out queryparameters if we actually have one
                        sb.Append("<QueryParameters>");
                        bFirst = false;
                    }
                    sb.AppendFormat(cinfo, "<QueryParameter Name=\"{0}\">", rp.Name);
                    sb.AppendFormat(cinfo, "<Value>=Parameters!{0}</Value>", rp.Name);
                    sb.Append("</QueryParameter>");
                }
            }
            if (!bFirst)
                sb.Append("</QueryParameters>");
        }

        private bool DoReportPreview()
        {
            if (tbReportSyntax.Text.Length < 1)
            {
                if (!DoReportSyntax())
                    return false;
            }
            rdlViewer1.SourceRdl = tbReportSyntax.Text;
            return true;
        }

        private string GetDataProvider()
        {
            string type = string.Empty;

            if (this.cbConnectionTypes.SelectedIndex == 0)
                type = "SQL";
            else if (this.cbConnectionTypes.SelectedIndex == 1)
                type = "OLEDB";
            else if (this.cbConnectionTypes.SelectedIndex == 2)
                type = "Oracle";
            else
                type = "ODBC";

            this.ConnectionString = tbConnection.Text;

            return type;
        }

        private string GetDataConnection()
        {	// GetDataProvider must be called first to ensure the DataConnection is correct.
            return ConnectionString;
        }

        #endregion

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (!DoReportSyntax())
                return;
            DialogResult = DialogResult.OK;
            _ResultReport = tbReportSyntax.Text;
            this.Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            string tag = (string)tc.TabPages[tc.SelectedIndex].Tag;
            switch (tag)
            {
                case "type":	// nothing to do here
                    break;
                case "connect":	// nothing to do here
                    break;
                case "sql":		// obtain table and column information
                    DoSqlSchema();
                    break;
                case "group":	// obtain group by information using connection & sql
                    DoGrouping();
                    break;
                case "syntax":	// obtain report using connection, sql, 
                    DoReportSyntax();
                    break;
                case "preview":	// run report using generated report syntax
                    DoReportPreview();
                    break;
                default:
                    break;
            }
        }

        private void tvTablesColumns_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            tvTablesColumns_ExpandTable(e.Node);
        }

        private void tvTablesColumns_ExpandTable(TreeNode tNode)
        {
            if (tNode.Parent == null)	// Check for Tables or Views
                return;

            if (tNode.FirstNode.Text != "")	// Have we already filled it out?
                return;

            // Need to obtain the column information for the requested table/view
            // suppress redraw until tree view is complete
            tvTablesColumns.BeginUpdate();

            string sql = "SELECT * FROM " + NormalizeName(tNode.Text);
            List<SqlColumn> tColumns = DesignerUtility.GetSqlColumns(GetDataProvider(), GetDataConnection(), sql, null);
            bool bFirstTime = true;
            foreach (SqlColumn sc in tColumns)
            {
                if (bFirstTime)
                {
                    bFirstTime = false;
                    tNode.FirstNode.Text = sc.Name;
                }
                else
                    tNode.Nodes.Add(sc.Name);
            }

            tvTablesColumns.EndUpdate();
        }

        private void tbSQL_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))	// only accept text
                e.Effect = DragDropEffects.Copy;
        }

        private void tbSQL_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                tbSQL.SelectedText = (string)e.Data.GetData(DataFormats.Text);
        }

        private void tvTablesColumns_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TreeNode node = tvTablesColumns.GetNodeAt(e.X, e.Y);
            if (node == null || node.Parent == null)
                return;

            string dragText;
            if (tbSQL.Text == "")
            {
                if (node.Parent.Parent == null)
                {	// select table; generate full select for table
                    tvTablesColumns_ExpandTable(node);	// make sure we've obtained the columns

                    dragText = "SELECT ";
                    TreeNode next = node.FirstNode;
                    while (true)
                    {
                        dragText += NormalizeName(next.Text);
                        next = next.NextNode;
                        if (next == null)
                            break;
                        dragText += ", ";
                    }
                    dragText += (" FROM " + NormalizeName(node.Text));
                }
                else
                {	// select column; generate select of that column	
                    dragText = "SELECT " + NormalizeName(node.Text) + " FROM " + NormalizeName(node.Parent.Text);
                }
            }
            else
                dragText = node.Text;

            tvTablesColumns.DoDragDrop(dragText, DragDropEffects.Copy);
        }

        private string NormalizeName(string name)
        {
            // Routine ensures valid sql name
            //    bool bLetterOrDigit = true;

            //    for (int i = 0; i < name.Length && bLetterOrDigit; i++)
            //    {
            //        if (name[i] == '.')
            //        {
            //            //bool bLetterOrDigit = true;
            //        }						// allow names to have a "." for owner qualified tables
            //        else if (!Char.IsLetterOrDigit(name, i))
            //            bLetterOrDigit = false;
            //    }
            //    if (bLetterOrDigit)
            //        return name;
            //    else
            //        return "\"" + name + "\"";

            return name;
        }

        private void tbSQL_TextChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
            _ColumnList = null;			// get rid of any column list as well
            cbColumnList.Items.Clear();	// and clear out other places where columns show
            cbColumnList.Text = "";
            clbSubtotal.Items.Clear();
        }

        private void tbReportName_TextChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void tbReportDescription_TextChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void tbReportAuthor_TextChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void rbTable_CheckedChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax

            if (rbTable.Checked)
            {
                TabularGroup.Enabled = true;
            }
            else
            {
                TabularGroup.Enabled = false;
            }
        }

        private void rbList_CheckedChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void rbMatrix_CheckedChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void rbChart_CheckedChanged(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";	// when SQL changes get rid of report syntax
        }

        private void bAdd_Click(object sender, System.EventArgs e)
        {
            ReportParm rp = new ReportParm("newparm");
            int cur = this.lbParameters.Items.Add(rp);
            lbParameters.SelectedIndex = cur;
            this.tbParmName.Focus();
        }

        private void bRemove_Click(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;
            lbParameters.Items.RemoveAt(cur);
            if (lbParameters.Items.Count <= 0)
                return;
            cur--;
            if (cur < 0)
                cur = 0;
            lbParameters.SelectedIndex = cur;
        }

        private void lbParameters_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            tbParmName.Text = rp.Name;
            cbParmType.Text = rp.DataType;
            tbParmPrompt.Text = rp.Prompt;
            tbParmDefaultValue.Text = rp.DefaultValueDisplay;
            ckbParmAllowBlank.Checked = rp.AllowBlank;
            tbParmValidValues.Text = rp.ValidValuesDisplay;
            ckbParmAllowNull.Checked = rp.AllowNull;
        }

        private void lbParameters_MoveItem(int curloc, int newloc)
        {
            ReportParm rp = lbParameters.Items[curloc] as ReportParm;
            if (rp == null)
                return;

            lbParameters.BeginUpdate();
            lbParameters.Items.RemoveAt(curloc);
            lbParameters.Items.Insert(newloc, rp);
            lbParameters.SelectedIndex = newloc;
            lbParameters.EndUpdate();
        }

        private void tbParmName_TextChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            if (rp.Name == tbParmName.Text)
                return;

            rp.Name = tbParmName.Text;
            // text doesn't change in listbox; force change by removing and re-adding item
            lbParameters_MoveItem(cur, cur);
        }

        private void cbParmType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            rp.DataType = cbParmType.Text;
        }

        private void tbParmPrompt_TextChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            rp.Prompt = tbParmPrompt.Text;
        }

        private void ckbParmAllowNull_CheckedChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            rp.AllowNull = ckbParmAllowNull.Checked;
        }

        private void ckbParmAllowBlank_CheckedChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            rp.AllowBlank = ckbParmAllowBlank.Checked;
        }

        private void bParmUp_Click(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur <= 0)
                return;

            lbParameters_MoveItem(cur, cur - 1);
        }

        private void bParmDown_Click(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur + 1 >= lbParameters.Items.Count)
                return;

            lbParameters_MoveItem(cur, cur + 1);
        }

        private void tbParmDefaultValue_TextChanged(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            if (tbParmDefaultValue.Text.Length > 0)
            {
                if (rp.DefaultValue == null)
                    rp.DefaultValue = new List<string>();
                else
                    rp.DefaultValue.Clear();
                rp.DefaultValue.Add(tbParmDefaultValue.Text);
            }
            else
                rp.DefaultValue = null;

        }

        private void tbConnection_TextChanged(object sender, System.EventArgs e)
        {
            tvTablesColumns.Nodes.Clear();
        }

        private void emptyReportSyntax(object sender, System.EventArgs e)
        {
            tbReportSyntax.Text = "";		// need to generate another report
        }

        private void bMove_Click(object sender, System.EventArgs e)
        {
            if (tvTablesColumns.SelectedNode == null ||
                tvTablesColumns.SelectedNode.Parent == null)
                return;		// this is the Tables/Views node

            TreeNode node = tvTablesColumns.SelectedNode;
            string t = node.Text;
            if (tbSQL.Text == "")
            {
                if (node.Parent.Parent == null)
                {	// select table; generate full select for table
                    tvTablesColumns_ExpandTable(node);	// make sure we've obtained the columns

                    StringBuilder sb = new StringBuilder("SELECT ");
                    TreeNode next = node.FirstNode;
                    while (true)
                    {
                        sb.Append(NormalizeName(next.Text));
                        next = next.NextNode;
                        if (next == null)
                            break;
                        sb.Append(", ");
                    }
                    sb.Append(" FROM ");
                    sb.Append(NormalizeName(node.Text));
                    t = sb.ToString();
                }
                else
                {	// select column; generate select of that column	
                    t = "SELECT " + NormalizeName(node.Text) + " FROM " + NormalizeName(node.Parent.Text);
                }
            }

            tbSQL.SelectedText = t;
        }

        private void bValidValues_Click(object sender, System.EventArgs e)
        {
            int cur = lbParameters.SelectedIndex;
            if (cur < 0)
                return;

            ReportParm rp = lbParameters.Items[cur] as ReportParm;
            if (rp == null)
                return;

            DialogValidValues dvv = new DialogValidValues(rp.ValidValues);
            if (dvv.ShowDialog() != DialogResult.OK)
                return;
            rp.ValidValues = dvv.ValidValues;
            this.tbParmValidValues.Text = rp.ValidValuesDisplay;
        }

        private void cbConnectionTypes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (cbConnectionTypes.Text)
            {
                case "MSSqlServer":
                    tbConnection.Text = "Server=(local);DataBase=Northwind;Integrated Security=SSPI;Connect Timeout=5";
                    break;
                case "OleDBSupported":
                    this.tbConnection.Text = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=oleDb.mdb;User ID=Admin;Password=;";
                    break;
                case "Oracle":
                    tbConnection.Text = "User Id=SYSTEM;Password=tiger;Data Source=orcl";
                    break;
                case "ODBC":
                    tbConnection.Text = "dsn=world;UID=user;PWD=pswd;";
                    break;
            }

        }

        private void btnSetConnect_Click(object sender, EventArgs e)
        {
            DataConnectionDialog dialog = new DataConnectionDialog();
            dialog.DataSources.Add(Microsoft.Data.ConnectionUI.DataSource.SqlDataSource);
            dialog.DataSources.Add(Microsoft.Data.ConnectionUI.DataSource.OdbcDataSource);
            dialog.DataSources.Add(Microsoft.Data.ConnectionUI.DataSource.OracleDataSource);
            dialog.DataSources.Add(Microsoft.Data.ConnectionUI.DataSource.AccessDataSource);

            dialog.SelectedDataSource = Microsoft.Data.ConnectionUI.DataSource.SqlDataSource;
            dialog.SelectedDataProvider = Microsoft.Data.ConnectionUI.DataProvider.SqlDataProvider;

            if (DataConnectionDialog.Show(dialog).Equals(DialogResult.OK))
            {
                switch (dialog.SelectedDataSource.Name)
                {
                    case "MicrosoftSqlServer":
                        this.cbConnectionTypes.SelectedIndex = 0;
                        break;
                    case "MicrosoftAccess":
                        this.cbConnectionTypes.SelectedIndex = 1;
                        break;
                    case "Oracle":
                        this.cbConnectionTypes.SelectedIndex = 2;
                        break;
                    case "OdbcDsn":
                        this.cbConnectionTypes.SelectedIndex = 3;
                        break;
                }

                this.tbConnection.Text = dialog.ConnectionString;
            }
        }

        private void bTestConnection_Click(object sender, System.EventArgs e)
        {
            string cType = GetDataProvider();
            if (cType == null)
                return;

            if (DesignerUtility.TestConnection(cType, GetDataConnection()))
                MessageBox.Show("Connection successful!", "Test Connection");
        }

        private void DBConnection_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!DesignerUtility.TestConnection(this.GetDataConnection(), GetDataConnection()))
                e.Cancel = true;
        }
    }
}