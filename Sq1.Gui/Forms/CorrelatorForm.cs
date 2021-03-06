﻿using System;
using System.Windows.Forms;

using Sq1.Core.Sequencing;
using Sq1.Widgets;
using Sq1.Core;
using System.Threading.Tasks;

namespace Sq1.Gui.Forms {
	public partial class CorrelatorForm : DockContentImproved {
		ChartFormsManager chartFormsManager;
		public bool Initialized { get { return this.chartFormsManager != null; } }

		// INVOKED_BY_DOCKCONTENT.DESERIALIZE_FROM_XML
		public CorrelatorForm() {
			InitializeComponent();
		}

		// INVOKED_AT_USER_CLICK
		public CorrelatorForm(ChartFormsManager chartFormManagerPassed) : this () {
			this.Initialize(chartFormManagerPassed);
			//ERASES_LINE_IN_DOCK_CONTENT_XML_IF_WITHOUT_IGNORING this.Disposed += this.LivesimForm_Disposed;
			this.FormClosing += new FormClosingEventHandler(this.correlatorForm_FormClosing);
			this.FormClosed += new FormClosedEventHandler(this.correlatorForm_FormClosed);
		}

		// http://www.codeproject.com/Articles/525541/Decoupling-Content-From-Container-in-Weifen-Luos
		// using ":" since "=" leads to an exception in DockPanelPersistor.cs
		protected override string GetPersistString() {
			return "Correlator:" + this.CorrelatorControl.GetType().FullName + ",ChartSerno:" + this.chartFormsManager.DataSnapshot.ChartSerno;
		}

		// INVOKED_AFTER_DOCKCONTENT.DESERIALIZE_FROM_XML
        internal void Initialize(ChartFormsManager chartFormsManagerPassed) {
            if (chartFormsManagerPassed == null) {
				string msg = "USE_DIFFERENT_VAR_NAME__DONT_PASS_CHART_FORMS_MANAGER=NULL:WindowTitlePullFromStrategy()_WILL_THROW";
				Assembler.PopupException(msg);
			}
			if (this.chartFormsManager == chartFormsManagerPassed) return;

			if (this.chartFormsManager != null) {
				this.chartFormsManager.Executor.Correlator
				.OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt
					-= new EventHandler<SequencedBacktestsEventArgs>(
						correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt);
			}
			this.chartFormsManager = chartFormsManagerPassed;
			this.chartFormsManager.Executor.Correlator
				.OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt
					-= new EventHandler<SequencedBacktestsEventArgs>(
						correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt);
			this.chartFormsManager.Executor.Correlator
				.OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt
					+= new EventHandler<SequencedBacktestsEventArgs>(
						correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt);

			this.CorrelatorControl.Initialize(this.chartFormsManager.Executor.Correlator);
			this.WindowTitlePullFromStrategy();
		}

		public void WindowTitlePullFromStrategy() {
			try {
				string windowTitle = "Correlator :: " + this.chartFormsManager.Strategy.WindowTitle;
				//if (this.chartFormsManager.Strategy.ActivatedFromDll == true) windowTitle += "-DLL";
				//if (this.chartFormsManager.ScriptEditedNeedsSaving) {
				//    windowTitle = ChartFormsManager.PREFIX_FOR_UNSAVED_STRATEGY_SOURCE_CODE + windowTitle;
				//}
				this.Text = windowTitle;
			} catch (Exception ex) {
				string msg = "WILL_CONTINUE_THOUGH";
				Assembler.PopupException(msg);
			}
		}

		// INVOKED_AT_USER_CLICK
		public void PopulateSequencedHistory(SequencedBacktests originalSequencedBacktests) {
			string msig = " //CorrelatorForm.PopulateSequencedHistory()";

			//MOVED_INTO_Initialize()
			//// UNSUBSCRIBE_FIRST__JUST_IN_CASE
			//this.chartFormsManager.Executor.Correlator
			//    .OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt
			//        -= new EventHandler<SequencedBacktestsEventArgs>(
			//            correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt);

			//this.chartFormsManager.Executor.Correlator
			//    .OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt
			//        += new EventHandler<SequencedBacktestsEventArgs>(
			//            correlator_OnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt);

			//v1
			this.CorrelatorControl.Initialize(originalSequencedBacktests
				, this.chartFormsManager.Executor.Strategy.RelPathAndNameForSequencerResults
				, originalSequencedBacktests.FileName);
			//v2 COULDNT_SET_SUBSET_PERCENTAGE
			//Task letGuiDraw = new Task(delegate() {
			//    this.CorrelatorControl.Initialize(originalOptimizationResults
			//        , this.chartFormsManager.Executor.Strategy.RelPathAndNameForSequencerResults
			//        , originalOptimizationResults.FileName);
			//    // WAS_ALREADY_INVOKED??? this.chartFormsManager.Executor.Correlator
			//    //	.RaiseOnSequencedBacktestsOriginalMinusParameterValuesUnchosenIsRebuilt();
			//});
			//letGuiDraw.ContinueWith(delegate {
			//    string msg = "TASK_THREW";
			//    Assembler.PopupException(msg + msig, letGuiDraw.Exception);
			//}, TaskContinuationOptions.OnlyOnFaulted);
			//letGuiDraw.Start();

			//this.chartFormsManager.SequencerFormConditionalInstance.SequencerControl.BacktestsReplaceWithCorrelated();
			//this.IsMdiContainer = true;	// trying to cheat DockPanel's "MdiContainers should be on TOP level" XML deserialization error
		}
	}
}
