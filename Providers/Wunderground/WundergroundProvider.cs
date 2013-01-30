using System;
using System.IO;
using System.Net;
using System.Xml.XPath;

namespace WeatherAPI.Providers.Wunderground {
	
	internal class WundergroundProvider : WeatherProvider, IWeather {
		
		private const string WG_API_FORMAT = "xml";
		private const string WG_API_URL = "http://api.wunderground.com/api/{0}/{1}/q/{2}.{3}";
		private const string WG_XPATH_HEADER = "/response/current_observation/{0}";
		
		private XPathDocument _xpath;
		private string _wg_api_key;

		public WundergroundProvider () : base() {
			if (IsAvailable()) {
				_wg_api_key = _dllConfig.AppSettings.Settings["WUNDERGROUND_API_KEY"].Value;
			}
		}
		
		public override bool IsAvailable() {
			return _dllConfig.AppSettings.Settings["WUNDERGROUND_API_KEY"] != null &&
					!String.IsNullOrEmpty(_dllConfig.AppSettings.Settings["WUNDERGROUND_API_KEY"].Value);
		}
		
		public override void Update() {
			const string features = "conditions";
			string urlLocation = getLocationForURL();

			string url = String.Format( WG_API_URL, _wg_api_key, features, urlLocation, WG_API_FORMAT );

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			string xml = new StreamReader(response.GetResponseStream()).ReadToEnd();
			
			_xpath = new XPathDocument(new StringReader(xml));
		}
		
		public override bool Supports(LocationSource source) { 
			return true;
		}
		
		public double DegreesCelcius {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "temp_c/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return Double.Parse(val.ToString());
			}
		}

		public double DegreesFahrenheit {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "temp_f/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return Double.Parse(val.ToString());
			}
		}

		public double WindSpeedMPH {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "wind_mph/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return Double.Parse(val.ToString());
			}
		}
		
		public double WindSpeedKPH {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "wind_kph/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return Double.Parse(val.ToString());
			}
		}
		
		public Direction WindDirection {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "wind_dir/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return (Direction)Enum.Parse(typeof(Direction), val.ToString());
			}
		}

		public double CloudCover {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "icon/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				return translateCloudCover(val.ToString());
			}
		}
		
		public double Precipitation {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "precip_today_metric/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);

				return Double.Parse(val.ToString());
			}
		}

		public double Humidity { 
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "relative_humidity/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);

				return Double.Parse( val.ToString().Trim( new char[] {' ', '%'} ) ) / 100.0;
			}
		}
		
		public WeatherCondition Conditions {
			get {
				string xpath = String.Format(WG_XPATH_HEADER, "weather/text()");
				object val = _xpath.CreateNavigator().SelectSingleNode(xpath);
				
				string condition = val.ToString();
				
				WeatherCondition currentCondition = translateConditions(condition);
				
				return currentCondition;
			}
		}
		
		protected WeatherCondition translateConditions (string condition)
		{
			WeatherCondition currentCondition = 0;

			foreach (string word in condition.Split( new char[]{ ' ' } )) {
				WeatherCondition tmpCondition;

				if (Enum.TryParse (word, true, out tmpCondition))
					currentCondition |= tmpCondition;
				else {
					switch (word) {
					case "hail":
						currentCondition |= WeatherCondition.Pellets;
						break;
					case "patches":
						currentCondition |= WeatherCondition.Patchy;
						break;
					case "shallow":
						currentCondition |= WeatherCondition.Light;
						break;
					default:
						break;
					}
				}
			}

			if (currentCondition == WeatherCondition.Patchy || currentCondition == WeatherCondition.Light
				|| currentCondition == WeatherCondition.Moderate || currentCondition == WeatherCondition.Heavy
				|| currentCondition == WeatherCondition.Torrential || currentCondition == WeatherCondition.Blizzard)
			{
				currentCondition = 0;
			}

			return currentCondition;
		}

		private double translateCloudCover (string iconText)
		{
			double cloudPercentage = 0.0;
			if (iconText.Contains("chance"))
				cloudPercentage = 0.8;
			else
			{
				switch (iconText) {
				case "clear":
				case "sunny":
					cloudPercentage = 0.0;
					break;
				case "hazy":
					cloudPercentage = 0.3;
					break;
				case "mostlysunny":
					cloudPercentage = 0.5;
					break;
				case "partlycloudy":
					cloudPercentage = 0.5;
					break;
				case "mostlycloudy":
				case "partlysunny":
					cloudPercentage = 0.7;
					break;
				case "cloudy":
					cloudPercentage = 0.8;
					break;
				case "flurries":
				case "sleet":
					cloudPercentage = 0.9;
					break;
				case "rain":
				case "fog":
				case "snow":
				case "tstorms":
				case "unknown":
					cloudPercentage = 1.0;
					break;
				}
			}

			return cloudPercentage;
		}

		private string getLocationForURL ()
		{
			string result = "";

			switch (this.Source) {
			case LocationSource.CityState:
				int commaIndex = this.Location.IndexOf(',');
				string city = this.Location.Substring(0, commaIndex);
				string state = this.Location.Substring(commaIndex + 1).Trim();
				result = String.Format ("{0}/{1}", state, city.Replace(' ', '_'));
				break;
			case LocationSource.LatitudeLongitude:
			case LocationSource.AirportCode:
			case LocationSource.ZipCode:
			default:
				result = Location;
				break;
			}

			return result;
		}
	}
}

