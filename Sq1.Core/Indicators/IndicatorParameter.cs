﻿using System;

using Sq1.Core.Correlation;
using Newtonsoft.Json;

namespace Sq1.Core.Indicators {
	public class IndicatorParameter {
		[JsonProperty]	public string			IndicatorName;
		[JsonIgnore]	public virtual string	FullName { get { return this.IndicatorName + "." + this.Name; } } // "MAslow.Period" for indicators, plain Name for StrategyParams
		
		[JsonProperty]	public string			Name;	// unlike user-editable ScriptParameter, IndicatorParameter.Name is compiled and remains constant (no need for Id)
		[JsonIgnore]	public string			ReasonToClone;
		[JsonProperty]	public double			ValueMin;
		[JsonProperty]	public double			ValueMax;
		[JsonProperty]	public double			ValueIncrement;
		[JsonProperty]	public double			ValueCurrent;
		[JsonIgnore]	public int				ValueCurrentAsInteger { get { return (int) Math.Round(this.ValueCurrent); } }
		[JsonProperty]	public bool				WillBeSequenced;
		[JsonProperty]	public int				NumberOfRuns { get {
				// got "Arithmetic opertation resulted in an overflow" when a Script wasn't built (IndicatorParameters restored from StrategyContext?)
				int ret = 1;
				if (this.ValueIncrement <= 0.0) return ret;
				if (double.IsNaN(this.ValueMax)) return ret;
				if (double.IsNaN(this.ValueMin)) return ret;
				if (double.IsNaN(this.ValueIncrement)) return ret;
				try {
					ret = (int)Math.Floor(Math.Abs((this.ValueMax - this.ValueMin)) / this.ValueIncrement) + 1;
				} catch (Exception ex) {
					Assembler.PopupException("fix NaNs and overflow", ex);
				}
				return ret;
			} }

		//public string ValueString;
		//public BarScaleInterval ValueBarScaleInterval;

		[JsonProperty]	public bool		BorderShown;
		[JsonProperty]	public bool		NumericUpdownShown;
		[JsonProperty]	public CorrelatorOneParameterSnapshot	CorrelatorSnap;

		// DESPITE_NOT_INVOKED_EXPLICITLY__I_GUESS_INITIALIZING_VALUES_USING_OTHER_CONSTRUCTOR_MAY_CORRUPT_JSON_DESERIALIZATION
		public IndicatorParameter() {
			BorderShown = false;
			NumericUpdownShown = true;
			IndicatorName = "NOT_ATTACHED_TO_ANY_INDICATOR_YET will be replaced in Indicator.cs:118.ParametersByNameParametersByName";
			CorrelatorSnap = new CorrelatorOneParameterSnapshot();
		}
		public IndicatorParameter(string name, double valueCurrent = double.NaN,
								  double valueMin = double.NaN, double valueMax = double.NaN, double valueIncrement = double.NaN) : this() {
//			if (name == null) {
//				if (this is IndicatorParameter) {
//					name = "INDICATOR_PARAMETER_NAME_NOT_INITIALIZED:" + new Random().Next(1000, 9999);
//				}
//				if (this is ScriptParameter) {
//					string msg = "SCRIPT_PARAMETER_NAME_CAN_BE_NULL ScriptParametersInitializedInDerivedConstructor will fix this by assigning my name to variable name I'm assigned to";
//				}
//			}
			Name			= name;
			ValueCurrent	= valueCurrent;
			ValueMin		= valueMin;
			ValueMax		= valueMax;
			ValueIncrement	= valueIncrement;
		}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", string value = "STRING_VALUE_NOT_INITIALIZED") {
		//	this.Name = name;
		//	this.ValueString = value;
		//}
		//public IndicatorParameter(string name = "NAME_NOT_INITIALIZED", BarScaleInterval value = null) {
		//	this.Name = name;
		//	this.ValueBarScaleInterval = value;
		//}
		public string ValidateSelf() {
			if (this.ValueMin		> this.ValueMax)	return "ValueMin["		+ this.ValueMin		+ "] > ValueMax["		+ this.ValueMax + "]";
			if (this.ValueCurrent	> this.ValueMax)	return "ValueCurrent["	+ this.ValueCurrent	+ "] > ValueMax["		+ this.ValueMax + "]";
			if (this.ValueCurrent	< this.ValueMin)	return "ValueCurrent["	+ this.ValueCurrent	+ "] < ValueCurrent["	+ this.ValueCurrent + "]";
			return null;
		}
		public override string ToString() {
			string ret = this.FullName + ":" + this.ValuesAsString;
			if (string.IsNullOrEmpty(this.ReasonToClone) == false) ret += " // " + this.ReasonToClone;
			return ret;
		}
		public string ValuesAsString { get {
				return this.ValueCurrent + "[" + this.ValueMin + ".." + this.ValueMax + "/" + this.ValueIncrement + "]";
			} }
		public bool AbsorbCurrentFixBoundariesIfChanged(IndicatorParameter ctxParamToAbsorbCurrentAndFixBoundaries) {
			this.WillBeSequenced	= ctxParamToAbsorbCurrentAndFixBoundaries.WillBeSequenced;
			this.BorderShown		= ctxParamToAbsorbCurrentAndFixBoundaries.BorderShown;
			this.NumericUpdownShown	= ctxParamToAbsorbCurrentAndFixBoundaries.NumericUpdownShown;
			this.CorrelatorSnap		= ctxParamToAbsorbCurrentAndFixBoundaries.CorrelatorSnap;

			bool ret = false;

			if (this.ValueCurrent != ctxParamToAbsorbCurrentAndFixBoundaries.ValueCurrent) {
				string msg = "we collapsed IndicatorParameters into a single instance thing; are we back to duplicates?...";
				//Debugger.Break();
				this.ValueCurrent = ctxParamToAbsorbCurrentAndFixBoundaries.ValueCurrent;
				ret = true;
			}
			if (this.ValueCurrent < this.ValueMin || this.ValueCurrent > this.ValueMax)  {
				string msg = "OBSERVED_AS_NEVER_HAPPENING";
				//Assembler.PopupException(msg);
				this.ValueCurrent = this.ValueMin;
			}
			if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueMin != this.ValueMin) {
				string msg = "OBSERVED_AS_NEVER_HAPPENING";
				//Assembler.PopupException(msg);
				ctxParamToAbsorbCurrentAndFixBoundaries.ValueMin = this.ValueMin;
			}
			if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueMax != this.ValueMax) {
				string msg = "OBSERVED_AS_NEVER_HAPPENING";
				//Assembler.PopupException(msg);
				ctxParamToAbsorbCurrentAndFixBoundaries.ValueMax = this.ValueMax;
			}
			if (ctxParamToAbsorbCurrentAndFixBoundaries.ValueIncrement != this.ValueIncrement) {
				string msg = "OBSERVED_AS_ALWAYS_HAPPENING";
				////Assembler.PopupException(msg, null, false);
				ctxParamToAbsorbCurrentAndFixBoundaries.ValueIncrement = this.ValueIncrement;
			}
			return ret;
		}
		// USED_TO_SEPARATE_LONG_LIVING_SCRIPT_INDICATOR_PARAMETER_INSTANCE__FROM_SWITCHING_CONTEXT_INDICATOR_SETTINGS
		public IndicatorParameter CloneAsIndicatorParameter(string reasonToClone) {
			IndicatorParameter ret = (IndicatorParameter)base.MemberwiseClone();
			ret.ReasonToClone = "CLONE[" + reasonToClone + "]_" + ret.ReasonToClone;
			return ret;
		}
		
		public void ShrinkForSerialization() {
			this.CorrelatorSnap = null;
		}

	}
}
