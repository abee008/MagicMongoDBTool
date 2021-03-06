﻿using MagicMongoDBTool.Module;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MagicMongoDBTool
{
    public partial class ConditionPanel : UserControl
    {
        /// <summary>
        /// 条件输入器数量
        /// </summary>
        private byte _conditionCount = 0;
        /// <summary>
        /// 条件输入器位置
        /// </summary>
        private Point _conditionPos = new Point(5, 0);
        /// <summary>
        /// 当前数据集的字段列表
        /// </summary>
        public List<String> ColumnList = new List<String>();
        public ConditionPanel()
        {
            InitializeComponent();
            if (SystemManager.GetCurrentCollection() != null)
            {
                ColumnList = MongoDBHelper.GetCollectionSchame(SystemManager.GetCurrentCollection());
            }
        }
        /// <summary>
        /// 新增条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddCondition()
        {
            _conditionCount++;
            ctlQueryCondition newCondition = new ctlQueryCondition();
            newCondition.Init(ColumnList);
            _conditionPos.Y += newCondition.Height;
            newCondition.Location = _conditionPos;
            newCondition.Name = "Condition" + _conditionCount.ToString();
            Controls.Add(newCondition);
        }
        /// <summary>
        /// 设置DataFilter
        /// </summary>
        public void SetCurrDataFilter(MongoDBHelper.DataViewInfo CurrentDataViewInfo)
        {
            //过滤条件
            for (int i = 0; i < _conditionCount; i++)
            {
                ctlQueryCondition ctl = (ctlQueryCondition)Controls.Find("Condition" + (i + 1).ToString(), true)[0];
                if (ctl.IsSeted)
                {
                    CurrentDataViewInfo.mDataFilter.QueryConditionList.Add(ctl.ConditionItem);
                }
            }
        }
        /// <summary>
        /// 将条件转成UI
        /// </summary>
        /// <param name="NewDataFilter"></param>
        public void PutQueryToUI(DataFilter NewDataFilter)
        {
            String strErrMsg = String.Empty;
            List<String> ShowColumnList = new List<String>();
            foreach (String item in ColumnList)
            {
                ShowColumnList.Add(item);
            }
            //清除所有的控件
            List<DataFilter.QueryFieldItem> FieldList = NewDataFilter.QueryFieldList;
            foreach (DataFilter.QueryFieldItem queryFieldItem in NewDataFilter.QueryFieldList)
            {
                //动态加载控件
                if (!ColumnList.Contains(queryFieldItem.ColName))
                {
                    strErrMsg += "Display Field [" + queryFieldItem.ColName + "] is not exist in current collection any more" + System.Environment.NewLine;
                }
                else
                {
                    ShowColumnList.Remove(queryFieldItem.ColName);
                }
            }
            foreach (String item in ShowColumnList)
            {
                strErrMsg += "New Field" + item + "Is Append" + System.Environment.NewLine;
                //输出配置的初始化
                FieldList.Add(new DataFilter.QueryFieldItem(item));
            }
            Controls.Clear();
            _conditionPos = new Point(5, 0);
            _conditionCount = 0;
            foreach (DataFilter.QueryConditionInputItem queryConditionItem in NewDataFilter.QueryConditionList)
            {
                ctlQueryCondition newCondition = new ctlQueryCondition();
                newCondition.Init(ColumnList);
                _conditionPos.Y += newCondition.Height;
                newCondition.Location = _conditionPos;
                newCondition.ConditionItem = queryConditionItem;
                _conditionCount++;
                newCondition.Name = "Condition" + _conditionCount.ToString();
                Controls.Add(newCondition);

                if (!ColumnList.Contains(queryConditionItem.ColName))
                {
                    strErrMsg += queryConditionItem.ColName + "Query Condition Field is not exist in collection any more" + System.Environment.NewLine;
                }
            }

            if (strErrMsg != String.Empty)
            {
                MyMessageBox.ShowMessage("Load Exception", "A Exception is happened when loading", strErrMsg, true);
            }
        }
        /// <summary>
        /// 清除条件
        /// </summary>
        public void ClearCondition()
        {
            Controls.Clear();
            _conditionCount = 0;
            _conditionPos = new Point(5, 0);
            AddCondition();
        }

    }
}
