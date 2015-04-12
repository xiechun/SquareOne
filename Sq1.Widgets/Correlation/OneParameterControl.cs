﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;
using Sq1.Core;
using Sq1.Core.Sequencing;
using Sq1.Core.Correlation;

namespace Sq1.Widgets.Correlation {
	public partial class OneParameterControl : UserControl {
		OneParameterAllValuesAveraged parameter;

		Dictionary<ToolStripMenuItem, List<OLVColumn>>			columnsByFilter;
		Dictionary<ToolStripMenuItem, OLVColumn>				columnToSortDescendingByMaximizationMni;
		Dictionary<MaximizationCriterion, ToolStripMenuItem>	mniToCheckForMaximizationCriterion;
		MaximizationCriterion sortBy;

		private Correlator sequencer { get { return this.allParametersControl.Sequencer; } }
		private AllParametersControl allParametersControl;

		public OneParameterControl() {
			InitializeComponent();
			// in case Designer drops them and I won't have any column selector by colheader rightclick anymore
			//this.olv.AllColumns.Add(this.olvcParamValues);
			//this.olv.AllColumns.Add(this.olvcTotalPositions);
			//this.olv.AllColumns.Add(this.olvcTotalPositionsLocal);
			//this.olv.AllColumns.Add(this.olvcProfitPerPosition);
			//this.olv.AllColumns.Add(this.olvcProfitPerPositionLocal);
			//this.olv.AllColumns.Add(this.olvcNetProfit);
			//this.olv.AllColumns.Add(this.olvcNetProfitLocal);
			//this.olv.AllColumns.Add(this.olvcWinLoss);
			//this.olv.AllColumns.Add(this.olvcWinLossLocal);
			//this.olv.AllColumns.Add(this.olvcProfitFactor);
			//this.olv.AllColumns.Add(this.olvcProfitFactorLocal);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactor);
			//this.olv.AllColumns.Add(this.olvcRecoveryFactorLocal);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdown);
			//this.olv.AllColumns.Add(this.olvcMaxDrawdownLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinners);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveWinnersLocal);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosers);
			//this.olv.AllColumns.Add(this.olvcMaxConsecutiveLosersLocal);

			this.mniToCheckForMaximizationCriterion = new Dictionary<MaximizationCriterion,		ToolStripMenuItem>();
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.PositionsCount,		this.mniMaximiseDeltaTotalPositions);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.PositionAvgProfit,	this.mniMaximiseDeltaProfitPerPosition);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.NetProfit,			this.mniMaximiseDeltaNet);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.WinLossRatio,			this.mniMaximiseDeltaWinLoss);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.ProfitFactor,			this.mniMaximiseDeltaProfitFactor);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.RecoveryFactor,		this.mniMaximiseDeltaRecoveryFactor);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxDrawDown,			this.mniMaximiseDeltaMaxDrawdown);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxConsecWinners,		this.mniMaximiseDeltaMaxConsecutiveWinners);
			this.mniToCheckForMaximizationCriterion.Add(MaximizationCriterion.MaxConsecLosers,		this.mniMaximiseDeltaMaxConsecutiveLosers);

			this.buildMniFilteringAfterInitializeComponent();
			this.buildMniSortingAfterInitializeComponent();
		}
		void buildMniSortingAfterInitializeComponent() {
			this.columnToSortDescendingByMaximizationMni = new Dictionary<ToolStripMenuItem, OLVColumn>();

			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaTotalPositions,			this.olvcTotalPositionsDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaProfitPerPosition,		this.olvcProfitPerPositionDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaNet,						this.olvcNetProfitDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaWinLoss,					this.olvcWinLossDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaProfitFactor,				this.olvcProfitFactorDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaRecoveryFactor,			this.olvcRecoveryFactorDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxDrawdown,				this.olvcMaxDrawdownDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxConsecutiveWinners,	this.olvcMaxConsecutiveWinnersDelta);
			this.columnToSortDescendingByMaximizationMni.Add(this.mniMaximiseDeltaMaxConsecutiveLosers,		this.olvcMaxConsecutiveLosersDelta);

			// sort by column that is Selected in Designer
			foreach (ToolStripMenuItem mni in this.columnsByFilter.Keys) {
				List<OLVColumn> columns = this.columnsByFilter[mni];
				foreach (OLVColumn column in columns) column.IsVisible = mni.Checked;
			}
			this.olv.RebuildColumns();

			this.mniMaximiseDeltaTotalPositions			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaProfitPerPosition		.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaNet					.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaWinLoss				.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaProfitFactor			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaRecoveryFactor			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxDrawdown			.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxConsecutiveWinners	.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
			this.mniMaximiseDeltaMaxConsecutiveLosers	.Click += new EventHandler(mniMaximizationKPISortDescendingBy_Click);
		}

		void checkOneMniForMaximizationCriterionUncheckOthers(ToolStripMenuItem mniToCheck) {
			mniToCheck.Checked = true;
			foreach (ToolStripMenuItem mniEachDisableCheckedUncheckOthers in this.columnToSortDescendingByMaximizationMni.Keys) {
				if (mniEachDisableCheckedUncheckOthers == mniToCheck) {
					//DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = false;
					continue;
				}
				//DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = true;
				mniEachDisableCheckedUncheckOthers.Checked = false;
			}
		}

		void buildMniFilteringAfterInitializeComponent() {
			this.columnsByFilter = new Dictionary<ToolStripMenuItem, List<OLVColumn>>();

			this.mniShowAllBacktestedParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowAllBacktestedParams, new List<OLVColumn>() {
				this.olvcTotalPositionsGlobal,
				this.olvcProfitPerPositionGlobal,
				this.olvcNetProfitGlobal,
				this.olvcWinLossGlobal,
				this.olvcProfitFactorGlobal,
				this.olvcRecoveryFactorGlobal,
				this.olvcMaxDrawdownGlobal,
				this.olvcMaxConsecutiveWinnersGlobal,
				this.olvcMaxConsecutiveLosersGlobal
				});

			this.mniShowChosenParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowChosenParams, new List<OLVColumn>() {
				this.olvcTotalPositionsLocal,
				this.olvcProfitPerPositionLocal,
				this.olvcNetProfitLocal,
				this.olvcWinLossLocal,
				this.olvcProfitFactorLocal,
				this.olvcRecoveryFactorLocal,
				this.olvcMaxDrawdownLocal,
				this.olvcMaxConsecutiveWinnersLocal,
				this.olvcMaxConsecutiveLosersLocal
				});

			this.mniShowDeltasBtwAllAndChosenParams.Click += new EventHandler(this.mniShowColumnByFilter_Click);
			columnsByFilter.Add(this.mniShowDeltasBtwAllAndChosenParams, new List<OLVColumn>() {
				this.olvcTotalPositionsDelta,
				this.olvcProfitPerPositionDelta,
				this.olvcNetProfitDelta,
				this.olvcWinLossDelta,
				this.olvcProfitFactorDelta,
				this.olvcRecoveryFactorDelta,
				this.olvcMaxDrawdownDelta,
				this.olvcMaxConsecutiveWinnersDelta,
				this.olvcMaxConsecutiveLosersDelta
				});

			// hide columns that aren't Checked in Designer
			foreach (ToolStripMenuItem mni in this.columnsByFilter.Keys) {
				List<OLVColumn> columns = this.columnsByFilter[mni];
				foreach (OLVColumn column in columns) column.IsVisible = mni.Checked;
			}
			this.olv.RebuildColumns();
		}

		public OneParameterControl(AllParametersControl allParametersControl, OneParameterAllValuesAveraged parameter) : this() {
			this.allParametersControl = allParametersControl;
			this.parameter = parameter;
			this.olvcParamValues.Text = this.parameter.ParameterName;
			this.olvAllValuesForOneParamCustomize();
			this.olv.SetObjects(this.parameter.AllValuesWithArtificials);
			parameter.OnParameterRecalculatedLocalsAndDeltas += new EventHandler<OneParameterAllValuesAveragedEventArgs>(parameter_ParameterRecalculatedLocalsAndDeltas);
		}

		void Initialize() {
			if (this.mniToCheckForMaximizationCriterion.ContainsKey(this.parameter.MaximizationCriterion)) {
				ToolStripMenuItem mniToCheck = this.mniToCheckForMaximizationCriterion[this.parameter.MaximizationCriterion];
				this.checkOneMniForMaximizationCriterionUncheckOthers(mniToCheck);
				OLVColumn sortBy = this.columnToSortDescendingByMaximizationMni[mniToCheck];
				this.olv.Sort(sortBy, SortOrder.Descending);
			}

			this.KPIsLocalRecalculateDone_refreshOLV();
		}

		internal void KPIsLocalRecalculateDone_refreshOLV() {
			this.olv.SetObjects(this.parameter.AllValuesWithArtificials, true);
			this.olv.UseWaitCursor = false;

			//NO_NEED_TO_RESORT_ONCE_SORTED
			//if (this.mniMaximiseDeltaAutoRunAfterSequencerFinished.Checked == false) return;
			//bool firstCheckedFound = false;
			//foreach (ToolStripMenuItem mniEachDisableCheckedUncheckOthers in this.columnToSortDescendingByMaximizationKPI.Keys) {
			//    if (firstCheckedFound == false && mniEachDisableCheckedUncheckOthers.Checked) {
			//        firstCheckedFound = true;
			//        //DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = false;
			//        OLVColumn sortBy = this.columnToSortDescendingByMaximizationKPI[mniEachDisableCheckedUncheckOthers];
			//        this.olvAllValuesForOneParam.Sort(sortBy, SortOrder.Descending);
			//        continue;
			//    }
			//    //DISABLED_HAS_NO_TICK_WEIRD mniEachDisableCheckedUncheckOthers.Enabled = true;
			//    mniEachDisableCheckedUncheckOthers.Checked = false;
			//}
		}

	}
}