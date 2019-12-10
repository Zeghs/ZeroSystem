using System;
using System.Collections.Generic;

namespace PowerLanguage {
	/// <summary>
	///   週期定義結構
	/// </summary>
	public struct Resolution {
		/// <summary>
		///   最小基礎週期總秒數
		/// </summary>
		public const int MIN_BASE_TOTALSECONDS = 60;

		/// <summary>
		///   最大基礎週期總秒數
		/// </summary>
		public const int MAX_BASE_TOTALSECONDS = 86400;

		/// <summary>
		///   取得基礎周期定義結構值
		/// </summary>
		/// <param name="value">當前週期定義結構</param>
		/// <returns>返回值: 基礎周期定義結構</returns>
		public static Resolution GetBaseValue(Resolution value) {
			if (value.TotalSeconds < MAX_BASE_TOTALSECONDS) {
				return new Resolution(EResolution.Minute, 1);
			} else {
				return new Resolution(EResolution.Day, 1);
			}
		}
		
		/// <summary>
		///   取得最接近 time 的週期
		/// </summary>
		/// <param name="periods">週期列表</param>
		/// <param name="time">time 結構</param>
		/// <returns>返回值: true=須建立新的周期, false=尚在週期範圍內</returns>
		public static bool GetNearestPeriod(List<DateTime> periods, ref DateTime time) {
			bool bRet = false;
			int iIndex = periods.Count - 1;
			if (iIndex >= 0) {
				DateTime cPrevious = time;
				for (int i = iIndex; i >= 0; i--) {
					DateTime cPeriod = periods[i];
					if (cPeriod >= time) {
						periods.RemoveAt(i);
						cPrevious = cPeriod;
						bRet = true;
					} else  {
						time = cPrevious;
						break;
					}
				}
			}
			return bRet;
		}

		/// <summary>
		///   取得最接近 time 的週期
		/// </summary>
		/// <param name="periods">週期佇列</param>
		/// <param name="time">time 結構</param>
		/// <returns>返回值: true=須建立新的 Bars 周期, false=尚在 Bars 週期範圍內</returns>
		public static bool GetNearestPeriod(Queue<DateTime> periods, ref DateTime time) {
			bool bRet = false;
			DateTime cPeriod = time;
			while (periods.Count > 0) {
				cPeriod = periods.Peek();
				if (time < cPeriod) {
					break;
				} else {
					periods.Dequeue();
					bRet = true;
				}
			}
			time = cPeriod;
			return bRet;
		}

		private static int Convert(EResolution type, int size) {
			switch (type) {
				case EResolution.Minute:
					return size * MIN_BASE_TOTALSECONDS;
				case EResolution.Hour:
					return size * 3600;
				case EResolution.Day:
					return size * MAX_BASE_TOTALSECONDS;
				case EResolution.Week:
					return size * MAX_BASE_TOTALSECONDS * 7;
				case EResolution.Month:
					return size * MAX_BASE_TOTALSECONDS * 31;
			}
			return MIN_BASE_TOTALSECONDS;
		}

		private int __iSize;
		private int __iRate;
		private bool __bModulo;
		private double __dUnit;
		private int __iTotalSeconds;
		private int __iSessionCount;
		private EResolution __cType;
		private TimeSpan __cEndTime;
		private TimeSpan __cStartTime;

		/// <summary>
		///   [取得/設定] 週期大小
		/// </summary>
		public int Size {
			get {
				return __iSize;
			}

			set {
				__iSize = value;
			}
		}

		/// <summary>
		///   [取得] 本周期變為基礎周期所需要的放大比率
		/// </summary>
		public int Rate {
			get {
				return __iRate;
			}
		}

		/// <summary>
		///   [取得] 時間週期的總秒數
		/// </summary>
		public int TotalSeconds {
			get {
				return __iTotalSeconds;
			}
		}

		/// <summary>
		///   [取得/設定] 週期定義類型
		/// </summary>
		public EResolution Type {
			get {
				return __cType;
			}

			set {
				__cType = value;
			}
		}

		/// <summary>
		///   初始化
		/// </summary>
		/// <param name="type">週期定義類型</param>
		/// <param name="size">週期大小</param>
		public Resolution(EResolution type, int size) {
			__cType = type;
			__iSize = size;
			
			__iRate = 1;
			__dUnit = 1;
			__bModulo = false;
			__iSessionCount = 0;
			__iTotalSeconds = Convert(type, size);
			__cEndTime = TimeSpan.Zero;
			__cStartTime = TimeSpan.Zero;
		}

		/// <summary>
		///   計算週期放大比率
		/// </summary>
		/// <param name="startTime">開盤時間</param>
		/// <param name="endTime">收盤時間</param>
		/// <param name="sessionCount">開收盤的天數</param>
		public void CalculateRate(TimeSpan startTime, TimeSpan endTime, int sessionCount) {
			__cStartTime = startTime;
			__cEndTime = endTime;
			__iSessionCount = sessionCount;

			if (__cType < EResolution.Day) {
				int iTotalSeconds = (int) (endTime - startTime).TotalSeconds;
				int iBaseUnit = iTotalSeconds / Resolution.MIN_BASE_TOTALSECONDS;
				__dUnit = iTotalSeconds / __iTotalSeconds;
				if (iTotalSeconds % __iTotalSeconds > 0) {
					++__dUnit;
					__bModulo = true;
				}
				__iRate = (int) (iBaseUnit / __dUnit);
			} else {
				__iRate = __iTotalSeconds / Resolution.MAX_BASE_TOTALSECONDS;
				__dUnit = 1 / __iRate;
			}
		}

		/// <summary>
		///   計算出時間區間內的所有時間週期(求出即時資訊的周期)
		/// </summary>
		/// <param name="lastBarsDate">歷史資訊最後一根 Bars 的日期</param>
		/// <param name="today">今天的日期</param>
		/// <param name="expiration">期權到期日(其他商品沒有到期日限制)</param>
		/// <returns>返回值: 時間週期 Queue</returns>
		public Queue<DateTime> CalculateRealtimePeriods(DateTime lastBarsDate, DateTime today, DateTime expiration) {
			DateTime cFrom = (__iTotalSeconds < MAX_BASE_TOTALSECONDS) ? today : lastBarsDate;
			DateTime cTo = new DateTime(today.Year, today.Month, today.Day, __cEndTime.Hours, __cEndTime.Minutes, __cEndTime.Seconds);
			if (__cType == EResolution.Month) {  //如果以月為周期就在多加上一天(不然會被修正為1號, 計算出來的結果會失真)
				cFrom = lastBarsDate.AddSeconds(MAX_BASE_TOTALSECONDS);
			}
			
			if (today.Date == expiration.Date) {  //檢查是否今天為到期日(如果是到期日就重新設定收盤時間為到期日的收盤時間)
				cTo = new DateTime(today.Year, today.Month, today.Day, expiration.Hour, expiration.Minute, expiration.Second);
			}
			return new Queue<DateTime>(CalculatePeriods(cFrom, cTo));
		}

		/// <summary>
		///   計算出時間區間內的所有時間週期
		/// </summary>
		/// <param name="_from">起始日期</param>
		/// <param name="_to">終止日期</param>
		/// <returns>返回值: 時間週期列表</returns>
		public List<DateTime> CalculatePeriods(DateTime _from, DateTime _to) {
			bool bMonth = false;
			switch (__cType) {
				case EResolution.Week:
					int iWeek = (int) _from.DayOfWeek;
					if (iWeek > 1) {
						_from = _from.AddDays(-7 + (__iSessionCount - iWeek));
					}
					_from = new DateTime(_from.Year, _from.Month, _from.Day, __cEndTime.Hours, __cEndTime.Minutes, __cEndTime.Seconds);
					break;
				case EResolution.Month:
					bMonth = true;
					_from = new DateTime(_from.Year, _from.Month, 1, __cEndTime.Hours, __cEndTime.Minutes, __cEndTime.Seconds);
					break;
				default:
					if (__iTotalSeconds < MAX_BASE_TOTALSECONDS) {
						_from = new DateTime(_from.Year, _from.Month, _from.Day, __cStartTime.Hours, __cStartTime.Minutes, __cStartTime.Seconds);
					} else {
						_from = new DateTime(_from.Year, _from.Month, _from.Day, __cEndTime.Hours, __cEndTime.Minutes, __cEndTime.Seconds);
					}
					break;
			}

			List<DateTime> cResult = new List<DateTime>(2048);
			cResult.Add(_from);

			while (_to > _from) {
				if (bMonth) {
					_from = _from.AddMonths(__iSize);
					cResult.Add(_from.AddSeconds(-MAX_BASE_TOTALSECONDS));
				} else {
					_from = _from.AddSeconds(__iTotalSeconds);
					if (__iTotalSeconds < MAX_BASE_TOTALSECONDS) {
						if (_from.TimeOfDay >= __cEndTime) {
							if (__bModulo) {
								double dModulo = (_from.TimeOfDay - __cEndTime).TotalSeconds;
								_from = _from.AddSeconds(-dModulo);
							}
							cResult.Add(_from);

							_from = _from.AddSeconds(MAX_BASE_TOTALSECONDS);
							_from = new DateTime(_from.Year, _from.Month, _from.Day, __cStartTime.Hours, __cStartTime.Minutes, __cStartTime.Seconds);
							continue;
						}
					}
					cResult.Add(_from);
				}
			}
			return cResult;
		}

		/// <summary>
		///   將天數轉換為 Bars 個數
		/// </summary>
		/// <param name="_days">天數</param>
		/// <returns>返回值:轉換後的 Bars 個數</returns>
		public int ConvertDaysToBars(int _days) {
			return (int) (_days * __dUnit) + 1;
		}
	}
}  //280行