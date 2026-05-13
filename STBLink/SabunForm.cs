using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STBLink
{
    public partial class SabunForm : System.Windows.Forms.Form
    {
        private bool first = true;

        public SabunForm()
        {
            InitializeComponent();

            first = true;
        }

        public class NodeSoter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (!(x is TreeNode tx)) return 0;
                if (!(y is TreeNode ty)) return 0;
                if (tx.Tag == null) return 0;
                if (ty.Tag == null) return 0;
                if (tx.Tag.ToString() == "") return 0;
                if (ty.Tag.ToString() == "") return 0;

                return string.Compare(tx.Tag.ToString(), ty.Tag.ToString());
            }
        }


        /// <summary>
        /// Canonical STB category key strings shared with <see cref="ConvertForm.Chb_class.buzai"/>, switches in <c>fromSTB.cs</c>, and differential import (<c>FromSTB_v2_sabun.cs</c>).
        /// Values must remain these exact literals; UI text is supplied by <see cref="CategoryDisplayNames"/>.
        /// </summary>
        private static class CategoryKeys
        {
            internal const string Column = "柱";
            internal const string IntermediateColumn = "間柱";
            internal const string FoundationColumn = "基礎柱";
            internal const string Girder = "大梁";
            internal const string Beam = "小梁";
            internal const string CantileverGirder = "片持梁";
            internal const string CantileverBeam = "片持小梁";
            internal const string SBrace = "Sブレース";
            internal const string RcSlab = "RCスラブ";
            internal const string DeckPlate = "デッキプレート";
            internal const string PrecastSlab = "既製スラブ";
            internal const string Wall = "壁";
            internal const string FoundationMatPile = "基礎・布基礎・杭";
            internal const string FoundationSlab = "基礎スラブ";
        }

        private readonly List<string> SortOrder = new List<string>()
        {
            CategoryKeys.Column,
            CategoryKeys.IntermediateColumn,
            CategoryKeys.FoundationColumn,
            CategoryKeys.Girder,
            CategoryKeys.Beam,
            CategoryKeys.CantileverGirder,
            CategoryKeys.CantileverBeam,
            CategoryKeys.SBrace,
            CategoryKeys.RcSlab,
            CategoryKeys.DeckPlate,
            CategoryKeys.PrecastSlab,
            CategoryKeys.Wall,
            CategoryKeys.FoundationMatPile,
            CategoryKeys.FoundationSlab,
        };

        /// <summary>
        /// English UI labels for each category; <see cref="TreeNode.Name"/> holds the key from <see cref="CategoryKeys"/>.
        /// </summary>
        private static readonly Dictionary<string, string> CategoryDisplayNames = new Dictionary<string, string>
        {
            [CategoryKeys.Column] = "Column",
            [CategoryKeys.IntermediateColumn] = "Intermediate column",
            [CategoryKeys.FoundationColumn] = "Foundation column",
            [CategoryKeys.Girder] = "Girder",
            [CategoryKeys.Beam] = "Beam",
            [CategoryKeys.CantileverGirder] = "Cantilever girder",
            [CategoryKeys.CantileverBeam] = "Cantilever beam",
            [CategoryKeys.SBrace] = "S brace",
            [CategoryKeys.RcSlab] = "RC slab",
            [CategoryKeys.DeckPlate] = "Deck plate",
            [CategoryKeys.PrecastSlab] = "Precast slab",
            [CategoryKeys.Wall] = "Wall",
            [CategoryKeys.FoundationMatPile] = "Foundation / mat foundation / pile",
            [CategoryKeys.FoundationSlab] = "Foundation slab",
        };

        private static string GetCategoryEnglishLabel(string buzaiKey) =>
            CategoryDisplayNames.TryGetValue(buzaiKey, out var en) ? en : buzaiKey;

        private TreeNode AddCategoryNode(TreeNodeCollection parent, string buzaiKey)
        {
            var n = new TreeNode(GetCategoryEnglishLabel(buzaiKey)) { Name = buzaiKey };
            parent.Add(n);
            n.Checked = true;
            return n;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (first)
            {
                // Form title
                this.Text = RevitLNK.formtitle + " Conversion type · instance selection " + Commons.GetVersion();

                for (int i = 0; i <= 1; ++i)
                {
                    TreeView treeView = i == 0 ? treeView1 : treeView2;

                    var target = new Dictionary<string, List<FromSTB_v2.ConvertCheck>>();

                    treeView.Nodes.Clear();


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecColumn_RC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecColumn_S != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecColumn_SRC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecColumn_CFT != null)
                    {
                        var n1 = AddCategoryNode(treeView.Nodes, CategoryKeys.Column);
                        n1.Tag = SortOrder.FindIndex(a => a == n1.Name).ToString("000");

                        var n2 = AddCategoryNode(treeView.Nodes, CategoryKeys.IntermediateColumn);
                        n2.Tag = SortOrder.FindIndex(a => a == n2.Name).ToString("000");

                        var n3 = AddCategoryNode(treeView.Nodes, CategoryKeys.FoundationColumn);
                        n3.Tag = SortOrder.FindIndex(a => a == n3.Name).ToString("000");

                        target.Add(n1.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n2.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n3.Name, new List<FromSTB_v2.ConvertCheck>());

                        if (ConvertForm.stb2.StbModel.StbSections.StbSecColumn_RC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecColumn_RC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }



                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());
                                bool foundation = false;
                                if (ConvertForm.stb2.StbModel.StbMembers.StbColumns != null)
                                {
                                    foundation = !(ConvertForm.stb2.StbModel.StbMembers.StbColumns.Any(a => a.id_section == section.id));
                                }
                                if (foundation)
                                {
                                    if (ConvertForm.stb2.StbModel.StbMembers.StbPosts != null)
                                    {
                                        foundation = !(ConvertForm.stb2.StbModel.StbMembers.StbPosts.Any(a => a.id_section == section.id));
                                    }
                                }
                                if (foundation)
                                {
                                    // Section used only for foundation columns, not for columns or intermediate columns
                                    if (ConvertForm.stb2.StbModel.StbMembers.StbFoundationColumns != null)
                                    {
                                        foundation = ConvertForm.stb2.StbModel.StbMembers.StbFoundationColumns.Any(a => a.id_section_FD == section.id || a.id_section_WR == section.id);
                                    }
                                }

                                if (foundation)
                                {
                                    if (SearchNode(n3.Nodes, nodename) == null)
                                    {
                                        AddNode(n3.Nodes, nodename);
                                        target[n3.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else if (section.kind_column == ST_BRIDGE_V2.StbSecColumn_Kind_column.POST)
                                {
                                    if (SearchNode(n2.Nodes, nodename) == null)
                                    {
                                        AddNode(n2.Nodes, nodename);
                                        target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else
                                {
                                    if (SearchNode(n1.Nodes, nodename) == null)
                                    {
                                        AddNode(n1.Nodes, nodename);
                                        target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecColumn_S != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecColumn_S)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.kind_column == ST_BRIDGE_V2.StbSecColumn_Kind_column.POST)
                                {
                                    if (SearchNode(n2.Nodes, nodename) == null)
                                    {
                                        AddNode(n2.Nodes, nodename);
                                        target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else
                                {
                                    if (SearchNode(n1.Nodes, nodename) == null)
                                    {
                                        AddNode(n1.Nodes, nodename);
                                        target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }

                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecColumn_SRC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecColumn_SRC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.kind_column == ST_BRIDGE_V2.StbSecColumn_Kind_column.POST)
                                {
                                    if (SearchNode(n2.Nodes, nodename) == null)
                                    {
                                        AddNode(n2.Nodes, nodename);
                                        target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else
                                {
                                    if (SearchNode(n1.Nodes, nodename) == null)
                                    {
                                        AddNode(n1.Nodes, nodename);
                                        target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }

                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecColumn_CFT != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecColumn_CFT)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.kind_column == ST_BRIDGE_V2.StbSecColumn_Kind_column.POST)
                                {
                                    if (SearchNode(n2.Nodes, nodename) == null)
                                    {
                                        AddNode(n2.Nodes, nodename);
                                        target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else
                                {
                                    if (SearchNode(n1.Nodes, nodename) == null)
                                    {
                                        AddNode(n1.Nodes, nodename);
                                        target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }

                            }
                        }
                    }


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecBeam_RC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecBeam_S != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecBeam_SRC != null)
                    {
                        var n1 = AddCategoryNode(treeView.Nodes, CategoryKeys.Girder);
                        n1.Tag = SortOrder.FindIndex(a => a == n1.Name).ToString("000");

                        var n2 = AddCategoryNode(treeView.Nodes, CategoryKeys.Beam);
                        n2.Tag = SortOrder.FindIndex(a => a == n2.Name).ToString("000");

                        var n3 = AddCategoryNode(treeView.Nodes, CategoryKeys.CantileverGirder);
                        n3.Tag = SortOrder.FindIndex(a => a == n3.Name).ToString("000");

                        var n4 = AddCategoryNode(treeView.Nodes, CategoryKeys.CantileverBeam);
                        n4.Tag = SortOrder.FindIndex(a => a == n4.Name).ToString("000");

                        target.Add(n1.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n2.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n3.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n4.Name, new List<FromSTB_v2.ConvertCheck>());


                        if (ConvertForm.stb2.StbModel.StbSections.StbSecBeam_RC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecBeam_RC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }



                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.isCanti)
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n4.Nodes, nodename) == null)
                                        {
                                            AddNode(n4.Nodes, nodename);
                                            target[n4.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n3.Nodes, nodename) == null)
                                        {
                                            AddNode(n3.Nodes, nodename);
                                            target[n3.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }
                                else
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n2.Nodes, nodename) == null)
                                        {
                                            AddNode(n2.Nodes, nodename);
                                            target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n1.Nodes, nodename) == null)
                                        {
                                            AddNode(n1.Nodes, nodename);
                                            target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }

                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecBeam_S != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecBeam_S)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.isCanti)
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n4.Nodes, nodename) == null)
                                        {
                                            AddNode(n4.Nodes, nodename);
                                            target[n4.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n3.Nodes, nodename) == null)
                                        {
                                            AddNode(n3.Nodes, nodename);
                                            target[n3.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }
                                else
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n2.Nodes, nodename) == null)
                                        {
                                            AddNode(n2.Nodes, nodename);
                                            target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n1.Nodes, nodename) == null)
                                        {
                                            AddNode(n1.Nodes, nodename);
                                            target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecBeam_SRC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecBeam_SRC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.isCanti)
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n4.Nodes, nodename) == null)
                                        {
                                            AddNode(n4.Nodes, nodename);
                                            target[n4.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n3.Nodes, nodename) == null)
                                        {
                                            AddNode(n3.Nodes, nodename);
                                            target[n3.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }
                                else
                                {
                                    if (section.kind_beam == ST_BRIDGE_V2.StbSecBeam_Kind_beam.BEAM)
                                    {
                                        if (SearchNode(n2.Nodes, nodename) == null)
                                        {
                                            AddNode(n2.Nodes, nodename);
                                            target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                    else
                                    {
                                        if (SearchNode(n1.Nodes, nodename) == null)
                                        {
                                            AddNode(n1.Nodes, nodename);
                                            target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                        }
                                    }
                                }

                            }
                        }
                    }


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecBrace_S != null)
                    {
                        var n = AddCategoryNode(treeView.Nodes, CategoryKeys.SBrace);
                        n.Tag = SortOrder.FindIndex(a => a == n.Name).ToString("000");

                        target.Add(n.Name, new List<FromSTB_v2.ConvertCheck>());


                        foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecBrace_S)
                        {
                            string fugo = section.name;
                            string guid = section.guid;

                            if (fugo == null || fugo == "") continue;
                            if (guid == null || guid == "") continue;


                            string typeName = "";

                            var eid = Data.GetStorageElementId(guid);
                            if (eid != null)
                            {
                                List<string> names = new List<string>();
                                for (int j = eid.Count - 1; j >= 0; --j)
                                {
                                    if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                    {
                                        names.Add($"{elm.Name}({elm.FamilyName})");
                                    }
                                    else
                                    {
                                        eid.RemoveAt(j);
                                    }
                                }
                                typeName = string.Join("/", names);
                            }

                            string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                            if (SearchNode(n.Nodes, nodename) == null)
                            {
                                AddNode(n.Nodes, nodename);
                                target[n.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                            }
                        }
                    }


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecSlab_RC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecSlabDeck != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecSlabPrecast != null)
                    {
                        var n1 = AddCategoryNode(treeView.Nodes, CategoryKeys.RcSlab);
                        n1.Tag = SortOrder.FindIndex(a => a == n1.Name).ToString("000");

                        var n2 = AddCategoryNode(treeView.Nodes, CategoryKeys.DeckPlate);
                        n2.Tag = SortOrder.FindIndex(a => a == n2.Name).ToString("000");

                        var n3 = AddCategoryNode(treeView.Nodes, CategoryKeys.PrecastSlab);
                        n3.Tag = SortOrder.FindIndex(a => a == n3.Name).ToString("000");

                        var n4 = AddCategoryNode(treeView.Nodes, CategoryKeys.FoundationSlab);
                        n4.Tag = SortOrder.FindIndex(a => a == n4.Name).ToString("000");

                        target.Add(n1.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n2.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n3.Name, new List<FromSTB_v2.ConvertCheck>());
                        target.Add(n4.Name, new List<FromSTB_v2.ConvertCheck>());


                        if (ConvertForm.stb2.StbModel.StbSections.StbSecSlab_RC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecSlab_RC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FloorType elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }



                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (section.isFoundation)
                                {
                                    if (SearchNode(n4.Nodes, nodename) == null)
                                    {
                                        AddNode(n4.Nodes, nodename);
                                        target[n4.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }
                                else
                                {
                                    if (SearchNode(n1.Nodes, nodename) == null)
                                    {
                                        AddNode(n1.Nodes, nodename);
                                        target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                    }
                                }

                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecSlabDeck != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecSlabDeck)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FloorType elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n2.Nodes, nodename) == null)
                                {
                                    AddNode(n2.Nodes, nodename);
                                    target[n2.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecSlabPrecast != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecSlabPrecast)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FloorType elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n3.Nodes, nodename) == null)
                                {
                                    AddNode(n3.Nodes, nodename);
                                    target[n3.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                    }


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecWall_RC != null)
                    {
                        var n = AddCategoryNode(treeView.Nodes, CategoryKeys.Wall);
                        n.Tag = SortOrder.FindIndex(a => a == n.Name).ToString("000");

                        target.Add(n.Name, new List<FromSTB_v2.ConvertCheck>());


                        foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecWall_RC)
                        {
                            string fugo = section.name;
                            string guid = section.guid;

                            if (fugo == null || fugo == "") continue;
                            if (guid == null || guid == "") continue;


                            string typeName = "";

                            var eid = Data.GetStorageElementId(guid);
                            if (eid != null)
                            {
                                List<string> names = new List<string>();
                                for (int j = eid.Count - 1; j >= 0; --j)
                                {
                                    if (Commons.doc.GetElement(eid[j]) is WallType elm)
                                    {
                                        names.Add($"{elm.Name}({elm.FamilyName})");
                                    }
                                    else
                                    {
                                        eid.RemoveAt(j);
                                    }
                                }
                                typeName = string.Join("/", names);
                            }

                            string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                            if (SearchNode(n.Nodes, nodename) == null)
                            {
                                AddNode(n.Nodes, nodename);
                                target[n.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                            }
                        }
                    }


                    if (ConvertForm.stb2.StbModel.StbSections.StbSecFoundation_RC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecPile_RC != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecPile_S != null ||
                        ConvertForm.stb2.StbModel.StbSections.StbSecPileProduct != null)
                    {
                        var n1 = AddCategoryNode(treeView.Nodes, CategoryKeys.FoundationMatPile);
                        n1.Tag = SortOrder.FindIndex(a => a == n1.Name).ToString("000");

                        target.Add(n1.Name, new List<FromSTB_v2.ConvertCheck>());


                        if (ConvertForm.stb2.StbModel.StbSections.StbSecFoundation_RC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecFoundation_RC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }



                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n1.Nodes, nodename) == null)
                                {
                                    AddNode(n1.Nodes, nodename);
                                    target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecPile_RC != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecPile_RC)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n1.Nodes, nodename) == null)
                                {
                                    AddNode(n1.Nodes, nodename);
                                    target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecPile_S != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecPile_S)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n1.Nodes, nodename) == null)
                                {
                                    AddNode(n1.Nodes, nodename);
                                    target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                        if (ConvertForm.stb2.StbModel.StbSections.StbSecPileProduct != null)
                        {
                            foreach (var section in ConvertForm.stb2.StbModel.StbSections.StbSecPileProduct)
                            {
                                string fugo = section.name;
                                string guid = section.guid;

                                if (fugo == null || fugo == "") continue;
                                if (guid == null || guid == "") continue;


                                string typeName = "";

                                var eid = Data.GetStorageElementId(guid);
                                if (eid != null)
                                {
                                    List<string> names = new List<string>();
                                    for (int j = eid.Count - 1; j >= 0; --j)
                                    {
                                        if (Commons.doc.GetElement(eid[j]) is FamilySymbol elm)
                                        {
                                            names.Add($"{elm.Name}({elm.FamilyName})");
                                        }
                                        else
                                        {
                                            eid.RemoveAt(j);
                                        }
                                    }
                                    typeName = string.Join("/", names);
                                }

                                string nodename = MakeNodeName(fugo, typeName, section.id.ToString());

                                if (SearchNode(n1.Nodes, nodename) == null)
                                {
                                    AddNode(n1.Nodes, nodename);
                                    target[n1.Name].Add(new FromSTB_v2.ConvertCheck(fugo, guid, section.id, eid, nodename));
                                }
                            }
                        }
                    }



                    treeView.TreeViewNodeSorter = new NodeSoter();
                    treeView.Sort();
                    treeView.CollapseAll();

                    if (i == 0)
                    {
                        FromSTB_v2.SabunTarget_T = target;
                    }
                    else
                    {
                        FromSTB_v2.SabunTarget_I = target;
                    }
                }
            }
            else
            {
                for (int i = 0; i <= 1; ++i)
                {
                    TreeView treeView = i == 0 ? treeView1 : treeView2;

                    var target = i == 0 ? FromSTB_v2.SabunTarget_T : FromSTB_v2.SabunTarget_I;

                    foreach (var t in target)
                    {
                        var n = SearchNode(treeView.Nodes, t.Key);
                        if (n == null)
                        {
                            n = AddCategoryNode(treeView.Nodes, t.Key);
                            n.Tag = SortOrder.FindIndex(a => a == n.Name).ToString("000");
                            foreach (var tt in t.Value)
                            {
                                AddNode(n.Nodes, tt.NodeName);
                            }
                        }
                        else
                        {
                            n.Checked = t.Value.Any(a => a.Check);
                            foreach (TreeNode nn in n.Nodes)
                            {
                                nn.Checked = t.Value.Find(a => a.NodeName == nn.Text).Check;
                            }
                        }
                    }

                    treeView.TreeViewNodeSorter = new NodeSoter();
                    treeView.Sort();
                    treeView.CollapseAll();
                }
            }



            // Remove categories that are excluded from conversion
            foreach (var c in ConvertForm.Chb_Checked)
            {
                if (!c.chbchecked)
                {
                    for (int i = 0; i <= 1; ++i)
                    {
                        TreeView treeView = i == 0 ? treeView1 : treeView2;

                        var n = SearchNode(treeView.Nodes, c.buzai);
                        if (n != null)
                        {
                            treeView.Nodes.Remove(n);
                        }
                    }
                }
            }


            first = false;
        }

        private TreeNode AddNode(TreeNodeCollection node, string name)
        {
            var n = node.Add(name);
            n.Checked = true;
            return n;
        }

        private static bool TreeNodeMatchesKey(TreeNode n, string lookup)
        {
            if (!string.IsNullOrEmpty(n.Name) && n.Name == lookup)
                return true;
            return n.Text == lookup;
        }

        private TreeNode SearchNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode n in nodes)
            {
                if (TreeNodeMatchesKey(n, name))
                {
                    return n;
                }

                if (n.Nodes != null && n.Nodes.Count > 0)
                {
                    var n2 = SearchNode(n.Nodes, name);
                    if (n2 != null)
                    {
                        return n2;
                    }
                }
            }

            return null;
        }


        private string MakeNodeName(string fugo, string typeName, string id)
        {
            if (typeName == "")
            {
                // Append id when several new marks share the same symbol (e.g. 1C1, 2C1, 3C1)
                typeName = $"New (id={id})";
            }

            string nodename = $"{fugo} : {typeName}";
            return nodename;
        }


        private void ChangeONOFF(TreeNodeCollection nodes, string name, bool onoff)
        {
            foreach (TreeNode n in nodes)
            {
                //// Other than root
                //if (n.Parent != null)
                {
                    if (name == "" || n.Text.Contains(name))
                    {
                        n.Checked = onoff;
                    }
                }


                if (n.Nodes != null && n.Nodes.Count > 0)
                {
                    ChangeONOFF(n.Nodes, name, onoff);
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ChangeONOFF(treeView1.Nodes, "", true);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ChangeONOFF(treeView1.Nodes, "", false);
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode n in e.Node.Nodes)
            {
                n.Checked = e.Node.Checked;
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            ChangeONOFF(treeView2.Nodes, "", true);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            ChangeONOFF(treeView2.Nodes, "", false);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SetTreeCheck(treeView1, FromSTB_v2.SabunTarget_T);
            SetTreeCheck(treeView2, FromSTB_v2.SabunTarget_I);


            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        private void SetTreeCheck(TreeView treeview, Dictionary<string, List<FromSTB_v2.ConvertCheck>> target)
        {
            foreach (var t in target)
            {
                t.Value.ForEach(a => a.Check = false);
            }

            foreach (TreeNode n in treeview.Nodes)
            {
                string categoryKey = string.IsNullOrEmpty(n.Name) ? n.Text : n.Name;
                if (target.ContainsKey(categoryKey))
                {
                    foreach (TreeNode n2 in n.Nodes)
                    {
                        if (n2.Checked)
                        {
                            if (target[categoryKey].Any(a => a.NodeName == n2.Text))
                            {
                                target[categoryKey].Find(a => a.NodeName == n2.Text).Check = n2.Checked;
                            }
                        }
                    }
                }
            }
        }


    }
}
