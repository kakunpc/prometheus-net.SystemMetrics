﻿using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on system load average
	/// </summary>
	public class LoadAverageCollector : ISystemMetricCollector
	{
		internal Gauge Load1 { get; private set; } = default!;
		internal Gauge Load5 { get; private set; } = default!;
		internal Gauge Load15 { get; private set; } = default!;

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			Load1 = Metrics.CreateGauge("node_load1", "Load average over the last minute.");
			Load5 = Metrics.CreateGauge("node_load5", "Load average over the last 5 minutes.");
			Load15 = Metrics.CreateGauge("node_load15", "Load average over the last 15 minutes.");
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			var loadAverage = new double[3];
			if (LinuxNative.getloadavg(loadAverage, 3) == 3)
			{
				Load1.Set(loadAverage[0]);
				Load5.Set(loadAverage[1]);
				Load15.Set(loadAverage[2]);
			}
		}
	}
}