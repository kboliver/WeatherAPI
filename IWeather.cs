//	Copyright 2012 John Luetke
//
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at
//
//		http://www.apache.org/licenses/LICENSE-2.0
//
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using System;

namespace WeatherAPI {
	
	/// <summary>
	/// Interface for obtaining weather information from an abstracted service
	/// </summary>
	public interface IWeather {
		
		/// <summary>
		/// Gets the degrees in Celcius at the time of the last update.
		/// </summary>
		/// <returns>
		/// The degrees in Celcius.
		/// </returns>
		double DegreesCelcius { get; }
		
		/// <summary>
		/// Gets the degress in Fahrenheit at the time of the last update.
		/// </summary>
		/// <returns>
		/// The degress in Fahrenheit.
		/// </returns>
		double DegreesFahrenheit { get ; }
		
		/// <summary>
		/// Gets the wind speed in miles per hour at the time of the last update.
		/// </summary>
		/// <returns>
		/// The wind speed in MPH.
		/// </returns>
		double WindSpeedMPH { get; }
		
		/// <summary>
		/// Gets the wind speed in kilometers per hour at the time of the last update.
		/// </summary>
		/// <returns>
		/// The wind speed in KPH.
		/// </returns>
		double WindSpeedKPH { get; }
		
		/// <summary>
		/// Gets the wind direction at the time of the last update.
		/// </summary>
		/// <returns>
		/// The wind direction.
		/// </returns>
		Direction WindDirection { get; }
		
		/// <summary>
		/// Gets the cloud cover at the time of the last update.
		/// </summary>
		/// <remarks>
		/// The value returned is a percentage. 0 is no clouds, 100 is overcast.
		/// </remarks>
		/// <returns>
		/// The cloud cover.
		/// </returns>
		double CloudCover { get; }
		
		/// <summary>
		/// Gets the percipitation in millimeters at the time of the last update.
		/// </summary>
		/// <returns>
		/// The percipitation in millimeters
		/// </returns>
		double Precipitation { get; }
		
		/// <summary>
		/// Gets the humidity at the time of the last update.
		/// </summary>
		/// <returns>
		/// The humidity.
		/// </returns>
		double Humidity { get; }
		
		/// <summary>
		/// Gets the weather conditions at the time of the last update.
		/// </summary>
		/// <returns>
		/// The weather conditions.
		/// </returns>
		WeatherCondition Conditions { get; }
	}
}

