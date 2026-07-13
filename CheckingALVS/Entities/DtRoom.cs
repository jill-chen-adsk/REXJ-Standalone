
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 部屋</summary>
    /// ================================================================================
    public class DtRoom : RvtExtApp.Entities.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpRoom _EntSpRoom;

        /// <summary>データ</summary>
        private System.Data.DataTable _Data;

        /// <summary>部屋</summary>
        private Collections.Generic.IList<Revit.DB.Architecture.Room> _Rooms;

        /// <summary>部屋面積</summary>
        private Collections.Generic.IList<string> _RoomAreas;

        /// <summary>部屋高さ</summary>
        private Collections.Generic.IList<string> _RoomHeights;

        /// <summary>部屋グループノードコレクション</summary>
        private System.Windows.Forms.TreeNodeCollection _RoomGroups;

        /// <summary>列名 計算グループ名</summary>
        private string _ColNameCalcGroupName;

        /// <summary>列名 部屋種類</summary>
        private string _ColNameRoomKind;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtRoom(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Elements cmpElements,
                      RvtExtApp.Components.Geometry cmpGeometry,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings
                      ) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpRoom = new RvtExtApp.Entities.SpRoom(cmpAttribute, cmpParameters, cmpSettings);

            if (_EntSpRoom.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpRoom.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpRoom.ErrDefName + "]";
            }
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            // ID
            data.Columns.Add(base.ColNameID, typeof(int));

            // レベル名
            data.Columns.Add(base.ColNameLevelName, typeof(string));

            // グループ名
            data.Columns.Add(ColNameGroupName, typeof(string));

            // 計算グループ名
            data.Columns.Add(ColNameCalcGroupName, typeof(string));

            // 部屋名
            data.Columns.Add(base.ColNameRoomName, typeof(string));

            // 部屋番号
            data.Columns.Add(base.ColNameRoomNo, typeof(string));

            // 面積
            data.Columns.Add(base.ColNameArea, typeof(string));

            // 部屋種類
            data.Columns.Add(ColNameRoomKind, typeof(string));

            // 必要係数
            data.Columns.Add(base.ColNameNecessaryCoefficient, typeof(string));

            // 平均天井高
            data.Columns.Add(base.ColNameAverageCeilingHeight, typeof(string));

            // 必要面積
            data.Columns.Add(base.ColNameNecessaryArea, typeof(string));

            // 合計有効面積
            data.Columns.Add(base.ColNameTotalUsableArea, typeof(string));

            // 判定
            data.Columns.Add(base.ColNameJudgment, typeof(string));
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemRoom"        >要素 - 部屋</param>
        /// <param name="roomArea"        >部屋面積</param>
        /// <param name="aveCeilingHeight">平均天井高</param>
        /// <param name="row"             >行データ</param>
        ///
        /// <history><p>2011/07/29 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public
        void GetData(Revit.DB.Architecture.Room elemRoom,
                     string roomArea,
                     string aveCeilingHeight,
                     ref System.Data.DataRow row)

        {
            string sValue = "";

            // 要素
            _EntSpRoom.CurrentElem = elemRoom;

            // ID
            row[base.ColNameID] = elemRoom.Id.ToString();

            // レベル名
            sValue = "";
            if (base.CmpElements.GetElementLevelName(elemRoom, ref sValue) == false)
            {
                sValue = "";
            }
            row[base.ColNameLevelName] = sValue;

            // グループ名
            sValue = _EntSpRoom.Group;
            if ((sValue == null) || (sValue == ""))
            {
                sValue = base.CmpAttribute.ResourceText("IDS_TXT_NOTHING");
            }
            row[ColNameGroupName] = sValue;

            // 計算グループ名
            row[ColNameCalcGroupName] = _EntSpRoom.CalcGroup;

            // 部屋名
            row[base.ColNameRoomName] = _EntSpRoom.RoomName;

            // 部屋番号
            row[base.ColNameRoomNo] = _EntSpRoom.RoomNo;

            // 面積
            row[base.ColNameArea] = UtilValue.Rounding(roomArea, _EntDtCmd.LegalAreaRoundingDecimal, _EntDtCmd.LegalAreaRoundingOpt);

            // 部屋種類
            row[ColNameRoomKind] = _EntSpRoom.Kind;

            // 必要係数
            sValue = "-";
            switch (base.CommandKind)
            {
                case 0:
                    sValue = GetNesCoeff(row[ColNameRoomKind].ToString());
                    break;

                case 1:
                    sValue = GetNesCoeff("0");
                    break;

                case 2:
                    sValue = GetNesCoeff("0");
                    break;
            }
            row[base.ColNameNecessaryCoefficient] = sValue;

            // 平均天井高
            row[base.ColNameAverageCeilingHeight] = UtilValue.Rounding(aveCeilingHeight, 1, 2);

            // 必要面積
            sValue = "-";
            row[base.ColNameNecessaryArea] = GetNesArea(row[ColNameArea].ToString(),
                                                        row[ColNameNecessaryCoefficient].ToString());
            // 合計有効面積
            row[base.ColNameTotalUsableArea] = "-";

            // 判定
            row[base.ColNameJudgment] = GetJudgment(row[ColNameNecessaryArea].ToString(),
                                                    row[ColNameTotalUsableArea].ToString());
        }

        /// ================================================================================
        /// <summary>データ取得(オーバーロード)</summary>
        ///
        /// <history><p>2011/07/29 Created GSA,Inc. Shinichi Ishii</p><history>
        /// ================================================================================
        public
        void GetData(RvtExtApp.Entities.DtCmd entDtCmd)
        {
            _EntDtCmd = entDtCmd;
            // データテーブル
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            if ((Rooms == null) || (RoomAreas == null) || (RoomHeights == null))
            {
                return;
            }

            for (int i = 0; i < Rooms.Count; ++i)
            {
                // 部屋面積
                string sArea = "0.0";
                if (i < RoomAreas.Count)
                {
                    sArea = RoomAreas[i];
                }

                // 平均天井高
                string sHeight = "0.0";
                if (i < RoomHeights.Count)
                {
                    sHeight = RoomHeights[i];
                }

                // データ取得
                System.Data.DataRow row = _Data.NewRow();
                GetData(Rooms[i], sArea, sHeight, ref row);

                _Data.Rows.Add(row);
            }

            // テーブルデータソート
            base.SortDataRoom(_Data);
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetData()
        {
            string sValue = "";

            if ((_Data != null) && (_Data.Rows.Count > 0))
            {
                for (int i = 0; i < _Data.Rows.Count; ++i)
                {
                    int id = 0;
                    sValue = _Data.Rows[i]["ID"].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        id = int.Parse(sValue);
                    }

                    Revit.DB.Architecture.Room room = base.CmpElements.GetRoom(id);
                    if (room != null)
                    {
                        _EntSpRoom.CurrentElem = room;

                        // 部屋種類
                        _EntSpRoom.Kind = _Data.Rows[i][ColNameRoomKind].ToString();

                        // グループ
                        string groupName = _Data.Rows[i][ColNameGroupName].ToString();
                        if (groupName == base.CmpAttribute.ResourceText("IDS_TXT_NOTHING"))
                        {
                            groupName = "";
                        }
                        _EntSpRoom.Group = groupName;

                        // 計算グループ
                        _EntSpRoom.CalcGroup = _Data.Rows[i][ColNameCalcGroupName].ToString();
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>必要面積取得</summary>
        ///
        /// <param name="area"    >面積</param>
        /// <param name="nesCoeff">必要係数</param>
        ///
        /// <returns>必要面積</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public
        string GetNesArea(string area, string nesCoeff)
        {
            string ret = null;

            double dArea = 0.0;
            double dCoeff = 0.0;
            double nesArea = 0.0;

            if (UtilValue.IsNumber(area) == true)
            {
                dArea = double.Parse(area);
            }

            if (UtilValue.IsNumber(nesCoeff) == true)
            {
                dCoeff = double.Parse(nesCoeff);
            }

            if (dCoeff != 0.0)
            {
                nesArea = dArea / dCoeff;
            }

            if (nesArea == 0.0)
            {
                ret = "-";
            }
            else
            {
                switch (base.CommandKind)
                {
                    case 0:
                        ret = UtilValue.Rounding(nesArea, _EntDtCmd.AreaToGetLightRoundingDecimal, _EntDtCmd.AreaToGetLightRoundingOpt);
                        break;

                    case 1:
                        ret = UtilValue.Rounding(nesArea, _EntDtCmd.AreaToBeSmokedRoundingDecimal, _EntDtCmd.AreaToBeSmokedRoundingOtp);
                        break;

                    case 2:
                        ret = UtilValue.Rounding(nesArea, _EntDtCmd.AreaToBeVentilatedRoundingDecimal, _EntDtCmd.AreaToBeVentilatedRoundingOtp);
                        break;
                }
            }

            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>必要面積取得(オーバーロード)</summary>
        ///
        /// <param name="roomID">部屋ID</param>
        ///
        /// <returns>必要面積</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GetNesArea(string roomID)
        {
            string ret = null;

            string sArea = UtilData.GetValueTableData(_Data, base.ColNameID, roomID.ToString(), base.ColNameArea);
            string sCofee = UtilData.GetValueTableData(_Data, base.ColNameID, roomID.ToString(), base.ColNameNecessaryCoefficient);

            ret = GetNesArea(sArea, sCofee);
            return ret;
        }

        /// ================================================================================
        /// <summary>判定取得(オーバーロード)</summary>
        ///
        /// <<param name="roomID">部屋ID</param>
        ///
        /// <returns>判定</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GetJudgment(string roomID)
        {
            string ret = null;

            string sNesArea = UtilData.GetValueTableData(_Data, base.ColNameID, roomID.ToString(), base.ColNameNecessaryArea);
            string sTotalUsableArea = UtilData.GetValueTableData(_Data, base.ColNameID, roomID.ToString(), base.ColNameTotalUsableArea);

            ret = base.GetJudgment(sNesArea, sTotalUsableArea);
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋グループ取得</summary>
        ///
        /// <param name="treeView">TreeViewコントロール</param>
        ///
        /// <history>2011/09/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetRoomGroup(ref System.Windows.Forms.TreeView treeView)
        {
            treeView.Nodes.Clear();
            System.Windows.Forms.TreeNode[] treeNodes;
            Collections.Generic.IList<string> groupNames = new Collections.Generic.List<string>();
            Collections.Generic.IList<System.Collections.Generic.IList<System.Data.DataRow>> groupNamesRooms = new Collections.Generic.List<System.Collections.Generic.IList<System.Data.DataRow>>();

            if (_Data == null)
            {
                return;
            }

            // グループ名
            foreach (System.Data.DataRow rowRoom in _Data.Rows)
            {
                // グループ名取得
                string groupName = rowRoom[ColNameGroupName].ToString();
                if (groupNames.Contains(groupName) == false)
                {
                    groupNames.Add(groupName);
                    Collections.Generic.IList<System.Data.DataRow> groupNameRooms = new Collections.Generic.List<System.Data.DataRow>();
                    groupNameRooms.Add(rowRoom);
                    groupNamesRooms.Add(groupNameRooms);
                }

                // 部屋取得
                else
                {
                    for (int i = 0; i < groupNames.Count; ++i)
                    {
                        if (groupNames[i] == groupName)
                        {
                            groupNamesRooms[i].Add(rowRoom);
                            break;
                        }
                    }
                }
            }

            // グループ名設定
            int cntGroupNames = groupNames.Count;
            if (cntGroupNames > 0)
            {
                treeNodes = new System.Windows.Forms.TreeNode[cntGroupNames];
                for (int i = 0; i < cntGroupNames; ++i)
                {
                    Collections.Generic.IList<System.Data.DataRow> groupNameRooms = groupNamesRooms[i];
                    System.Windows.Forms.TreeNode[] subTreeNodes = null;

                    // Set room name
                    // 部屋名設定
                    int cntGroupNameRooms = groupNameRooms.Count;
                    if (cntGroupNameRooms > 0)
                    {
                        subTreeNodes = new System.Windows.Forms.TreeNode[cntGroupNameRooms];

                        for (int j = 0; j < cntGroupNameRooms; ++j)
                        {
                            System.Data.DataRow rowRoom = groupNameRooms[j];
                            System.Windows.Forms.TreeNode treeNodeSub = new System.Windows.Forms.TreeNode(rowRoom[base.ColNameRoomName].ToString() +
                                                                                                          rowRoom[base.ColNameRoomNo].ToString());
                            subTreeNodes[j] = treeNodeSub;
                            subTreeNodes[j].Tag = rowRoom[base.ColNameID].ToString();
                        }
                    }
                    System.Windows.Forms.TreeNode treeNodeMain = new System.Windows.Forms.TreeNode(groupNames[i], subTreeNodes);
                    treeNodes[i] = treeNodeMain;
                }
                treeView.Nodes.AddRange(treeNodes);
                treeView.Sort();
                _RoomGroups = treeView.Nodes;
            }
        }

        /// ================================================================================
        /// <summary>部屋グループ追加</summary>
        ///
        /// <param name="treeView"  >TreeViewコントロール</param>
        /// <param name="groupName" >グループ名</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void AddRoomGroup(ref System.Windows.Forms.TreeView treeView, string groupName)
        {
            bool flag = true;
            foreach (System.Windows.Forms.TreeNode treeNode in treeView.Nodes)
            {
                if (treeNode.Parent == null)
                {
                    if (treeNode.Text == groupName)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag == true)
            {
                treeView.Nodes.Add(groupName);
                treeView.Sort();
                treeView.SelectedNode = treeView.Nodes[0];
            }
        }

        /// ================================================================================
        /// <summary>部屋グループ削除</summary>
        ///
        /// <param name="treeView"  >TreeViewコントロール</param>
        /// <param name="treeNode"  >Tree node</param>
        /// <param name="dgvRoom"   >部屋のDataGridView</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void DeleteRoomGroup(ref System.Windows.Forms.TreeView treeView,
                             System.Windows.Forms.TreeNode treeNode,
                             ref System.Windows.Forms.DataGridView dgvRoom)
        {
            System.Windows.Forms.TreeNodeCollection treeNodeCol = treeNode.Nodes;
            if (treeNodeCol.Count > 0)
            {
                for (int i = 0; i < treeNodeCol.Count; ++i)
                {
                    System.Windows.Forms.TreeNode treeNodeSub = treeNodeCol[i];
                    int roomId = int.Parse(treeNodeSub.Tag.ToString());
                    for (int j = 0; j < _Data.Rows.Count; ++j)
                    {
                        int roomRowId = int.Parse(_Data.Rows[j][ColNameID].ToString());
                        if (roomId == roomRowId)
                        {
                            System.Data.DataTable dummy = _Data.Copy();
                            dummy.Rows[j][ColNameGroupName] = base.CmpAttribute.ResourceText("IDS_TXT_NOTHING");
                            _Data = dummy.Copy();
                            dgvRoom.DataSource = _Data;
                            break;
                        }
                    }
                }
            }
            treeView.Nodes.Remove(treeNode);
            if (treeView.Nodes.Count > 0)
            {
                UpdateRoomGroup(ref treeView);
                treeView.Sort();
                treeView.SelectedNode = treeView.Nodes[0];
            }
        }

        /// ================================================================================
        /// <summary>部屋グループ移動</summary>
        ///
        /// <param name="treeView"  >TreeViewコントロール</param>
        /// <param name="treeNode"  >Tree node</param>
        /// <param name="toTreeNode">移動先のTree node</param>
        /// <param name="dgvRoom"   >部屋のDataGridView</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void MoveRoomGroup(ref System.Windows.Forms.TreeView treeView,
                           System.Windows.Forms.TreeNode treeNode,
                           System.Windows.Forms.TreeNode toTreeNode,
                           ref System.Windows.Forms.DataGridView dgvRoom)
        {
            // 移動先に追加
            string groupName = toTreeNode.Text;
            int roomId = int.Parse(treeNode.Tag.ToString());
            for (int i = 0; i < _Data.Rows.Count; ++i)
            {
                int roomRowId = int.Parse(_Data.Rows[i][ColNameID].ToString());
                if (roomId == roomRowId)
                {
                    System.Data.DataTable dummy = _Data.Copy();
                    dummy.Rows[i][ColNameGroupName] = groupName;
                    _Data = dummy.Copy();
                    dgvRoom.DataSource = _Data;
                    break;
                }
            }

            UpdateRoomGroup(ref treeView);
            treeView.Sort();
            treeView.SelectedNode = treeView.Nodes[0];
        }

        /// ================================================================================
        /// <summary>部屋グループ更新</summary>
        ///
        /// <param name="treeView">TreeViewコントロール</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void UpdateRoomGroup(ref System.Windows.Forms.TreeView treeView)
        {
            foreach (System.Windows.Forms.TreeNode treeNode in treeView.Nodes)
            {
                if (treeNode.Parent == null)
                {
                    string groupName = treeNode.Text;
                    System.Windows.Forms.TreeNodeCollection treeNodeCol = treeNode.Nodes;
                    if (treeNodeCol.Count > 0)
                    {
                        treeNodeCol.Clear();
                    }
                    foreach (System.Data.DataRow row in _Data.Rows)
                    {
                        if (row[ColNameGroupName].ToString() == groupName)
                        {
                            System.Windows.Forms.TreeNode treeNodeSub = treeNode.Nodes.Add(row[ColNameRoomName].ToString() +
                                                                                           row[ColNameRoomNo].ToString());
                            treeNodeSub.Tag = row[ColNameID].ToString();
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>部屋グループ集計</summary>
        ///
        /// <param name="groupName"             >グループ名</param>
        /// <param name="strGroupNecessaryArea" >必要面積</param>
        /// <param name="strGroupUsableArea"    >有効面積</param>
        /// <param name="strGroupJudgment"      >判定</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void TotalRoomGroup(string groupName,
                            ref string strGroupNecessaryArea,
                            ref string strGroupUsableArea,
                            ref string strGroupJudgment)
        {
            string sValue = "";

            strGroupNecessaryArea = "-";
            strGroupUsableArea = "-";
            strGroupJudgment = "-";

            // グループ名
            if (groupName == base.CmpAttribute.ResourceText("IDS_TXT_NOTHING"))
            {
                return;
            }
            strGroupJudgment = "NG";

            // 行数
            int rowsNum = _Data.Rows.Count;
            if (rowsNum == 0)
            {
                return;
            }

            // 値
            double dGroupNecessaryArea = 0.0;
            double dGroupUsableArea = 0.0;
            for (int i = 0; i < rowsNum; ++i)
            {
                // グループ名
                string roomGroupName = _Data.Rows[i][ColNameGroupName].ToString();
                if (groupName != roomGroupName)
                {
                    continue;
                }

                // 必要面積
                sValue = _Data.Rows[i][base.ColNameNecessaryArea].ToString();
                if (UtilValue.IsNumber(sValue) == true)
                {
                    dGroupNecessaryArea += double.Parse(sValue);
                }

                // 有効面積
                sValue = _Data.Rows[i][base.ColNameTotalUsableArea].ToString();
                if (UtilValue.IsNumber(sValue) == true)
                {
                    dGroupUsableArea += double.Parse(sValue);
                }
            }

            // 判定
            if (dGroupNecessaryArea != 0.0)
            {
                strGroupNecessaryArea = UtilValue.Rounding(dGroupNecessaryArea, 3, 2);
            }
            if (dGroupUsableArea != 0.0)
            {
                strGroupUsableArea = UtilValue.Rounding(dGroupUsableArea, 3, 2);
            }

            strGroupJudgment = GetJudgment(strGroupNecessaryArea, strGroupUsableArea);
        }

        /// ================================================================================
        /// <summary>グループ名取得</summary>
        ///
        /// <param name="roomId">部屋ID</param>
        ///
        /// <returns>グループ名</returns>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GetGroupName(string roomID)
        {
            return UtilData.GetValueTableData(_Data, ColNameID, roomID, ColNameGroupName);
        }

        /// ================================================================================
        /// <summary>部屋のグループ名を更新</summary>
        ///
        /// <param name="roomId"   >部屋ID</param>
        /// <param name="groupName">グループ名</param>
        ///
        /// <returns>更新できた場合は true</returns>
        /// ================================================================================
        public
        bool AssignRoomToGroup(string roomId, string groupName)
        {
            if (_Data == null || string.IsNullOrWhiteSpace(roomId))
                return false;

            return AssignRoomsToGroup(new[] { roomId }, groupName);
        }

        /// ================================================================================
        /// <summary>複数部屋のグループ名を更新</summary>
        ///
        /// <param name="roomIds"  >部屋ID</param>
        /// <param name="groupName">グループ名</param>
        ///
        /// <returns>更新できた場合は true</returns>
        /// ================================================================================
        public
        bool AssignRoomsToGroup(Collections.Generic.IEnumerable<string> roomIds, string groupName)
        {
            if (_Data == null || roomIds == null)
                return false;

            var roomIdSet = new Collections.Generic.HashSet<string>();
            foreach (string roomId in roomIds)
            {
                if (!string.IsNullOrWhiteSpace(roomId))
                    roomIdSet.Add(roomId);
            }

            if (roomIdSet.Count == 0)
                return false;

            System.Data.DataTable dummy = _Data.Copy();
            bool changed = false;
            for (int i = 0; i < dummy.Rows.Count; ++i)
            {
                string rowRoomId = dummy.Rows[i][ColNameID].ToString();
                if (!roomIdSet.Contains(rowRoomId))
                    continue;

                dummy.Rows[i][ColNameGroupName] = groupName;
                changed = true;
            }

            if (!changed)
                return false;

            _Data = dummy.Copy();
            return true;
        }

        /// ================================================================================
        /// <summary>対象部屋グループの部屋行を表示設定</summary>
        ///
        /// <param name="roomGroupName">部屋グループ名</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetVisbleRowsRoomsOfRoomGroup(string roomGroupName)
        {
            string colName = ColNameGroupName;
            string filterStr = "";
            filterStr = colName + " = " + "'" + roomGroupName + "'";
            _Data.DefaultView.RowFilter = filterStr;
            base.SortDataRoom(_Data);
        }

        /// ================================================================================
        /// <summary>対象部屋グループの部屋行を表示設定</summary>
        ///
        /// <param name="roomGroupName">部屋グループ名</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetVisbleRowsRoomsOfRoomGroup(string roomGroupName, ref System.Windows.Forms.DataGridView dgvRoom)
        {
            System.Data.DataTable dt = (System.Data.DataTable)dgvRoom.DataSource;
            string colName = ColNameGroupName;
            string filterStr = "";
            filterStr = colName + " = " + "'" + roomGroupName + "'";
            dt.DefaultView.RowFilter = filterStr;
            base.SortDataRoom(_Data);
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        RvtExtApp.Entities.SpRoom EntSpRoom
        {
            get
            {
                return _EntSpRoom;
            }
        }

        /// ================================================================================
        /// <summary>データ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
        }

        /// ================================================================================
        /// <summary>部屋グループノードコレクション</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Windows.Forms.TreeNodeCollection RoomGroups
        {
            get
            {
                return _RoomGroups;
            }
            set
            {
                _RoomGroups = value;
            }
        }

        /// ================================================================================
        /// <summary>部屋</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Architecture.Room> Rooms
        {
            get
            {
                return _Rooms;
            }
            set
            {
                _Rooms = value;
            }
        }

        /// ================================================================================
        /// <summary>部屋面積</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> RoomAreas
        {
            get
            {
                return _RoomAreas;
            }
            set
            {
                _RoomAreas = value;
            }
        }

        /// ================================================================================
        /// <summary>部屋高さ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> RoomHeights
        {
            get
            {
                return _RoomHeights;
            }
            set
            {
                _RoomHeights = value;
            }
        }

        /// ================================================================================
        /// <summary>列名 計算グループ名</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameCalcGroupName
        {
            get
            {
                if (_ColNameCalcGroupName == null)
                {
                    _ColNameCalcGroupName = base.CmpAttribute.ResourceText("IDS_COLNAME_CALCGROUPNAME");
                }
                return _ColNameCalcGroupName;
            }
        }

        /// ================================================================================
        /// <summary>列名 部屋種類</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameRoomKind
        {
            get
            {
                if (_ColNameRoomKind == null)
                {
                    _ColNameRoomKind = base.CmpAttribute.ResourceText("IDS_COLNAME_ROOMKIND");
                }
                return _ColNameRoomKind;
            }
        }

        #endregion Properties
    }
}