using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace WeatherApp
{
    // The following code is written mostly ;) by: https://github.com/6outtaTen/

    ////////////////////////////// CREDITS //////////////////////////////
    // Wind Icon: https://www.flaticon.com/free-icon/wind_959711?term=wind&page=1&position=1&page=1&position=1&related_id=959711&origin=tag
    // Pressure Gauge Icon: https://www.flaticon.com/free-icon/pressure-gauge_1839341?term=pressure&related_id=1839341
    // Thermometer Icon: https://www.flaticon.com/premium-icon/thermometer_2100100?term=thermometer&page=1&position=4&page=1&position=4&related_id=2100100&origin=tag
    // Humidity Icon: https://www.flaticon.com/premium-icon/humidity_2828582?term=humidity&page=1&position=14&page=1&position=14&related_id=2828582&origin=search
    // Clock Icon: https://www.flaticon.com/premium-icon/clock_2838590?term=clock&page=1&position=10&page=1&position=10&related_id=2838590&origin=search
    // Location Icon: https://www.flaticon.com/premium-icon/placeholder_186250?term=location&page=1&position=9&page=1&position=9&related_id=186250&origin=search

    ////////////////////////////// API'S ///////////////////////////////
    // API used for getting public ip address of the user: https://api.ipify.org/?format=json
    // API used for getting user's city based on the public ip address: https://ipdata.co/
    // API used for sunset and sunrise data: https://sunrise-sunset.org/
    // API used for weather data: https://www.weatherapi.com/
    // API used for air quality data: https://aqicn.org/

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            InitializeWeatherDataBasedOnUserLocation();
        }
 
        ////////////////////////////////////////// Classes used for storing data got from api's //////////////////////////////////////////
        public class WeatherData
        {
            public Location location { get; set; }
            public Current current { get; set; }
            public Forecast forecast { get; set; }
        }

        public class Location
        {
            public string name { get; set; }
            public string region { get; set; }
            public string country { get; set; }
            public float lat { get; set; }
            public float lon { get; set; }
            public string tz_id { get; set; }
            public int localtime_epoch { get; set; }
            public string localtime { get; set; }
        }

        public class Current
        {
            public float temp_c { get; set; }
            public Condition condition { get; set; }
            public float wind_kph { get; set; }
            public string wind_dir { get; set; }
            public float pressure_mb { get; set; }
            public float pressure_in { get; set; }
            public float precip_mm { get; set; }
            public float precip_in { get; set; }
            public int humidity { get; set; }
            public int cloud { get; set; }
            public float feelslike_c { get; set; }
            public float feelslike_f { get; set; }
            public float vis_km { get; set; }
            public float vis_miles { get; set; }
            public float uv { get; set; }
            public float gust_mph { get; set; }
            public float gust_kph { get; set; }
        }

        public class Condition
        {
            public string text { get; set; }
            public string icon { get; set; }
            public int code { get; set; }
        }

        public class Forecast
        {
            public Forecastday[] forecastday { get; set; }
        }

        public class Forecastday
        {
            public string date { get; set; }
            public int date_epoch { get; set; }
            public Day day { get; set; }
            public Astro astro { get; set; }
            public Hour[] hour { get; set; }
        }

        public class Day
        {
            public float maxtemp_c { get; set; }
            public float maxtemp_f { get; set; }
            public float mintemp_c { get; set; }
            public float mintemp_f { get; set; }
            public float avgtemp_c { get; set; }
            public float avgtemp_f { get; set; }
            public float maxwind_mph { get; set; }
            public float maxwind_kph { get; set; }
            public float totalprecip_mm { get; set; }
            public float totalprecip_in { get; set; }
            public float avgvis_km { get; set; }
            public float avgvis_miles { get; set; }
            public float avghumidity { get; set; }
            public int daily_will_it_rain { get; set; }
            public int daily_chance_of_rain { get; set; }
            public int daily_will_it_snow { get; set; }
            public int daily_chance_of_snow { get; set; }
            public Condition1 condition { get; set; }
            public float uv { get; set; }
        }

        public class Condition1
        {
            public string text { get; set; }
            public string icon { get; set; }
            public int code { get; set; }
        }

        public class Astro
        {
            public string sunrise { get; set; }
            public string sunset { get; set; }
            public string moonrise { get; set; }
            public string moonset { get; set; }
            public string moon_phase { get; set; }
            public string moon_illumination { get; set; }
        }


        public class IPData
        {
            public string ip { get; set; }
        }

        public class Hour
        {
            public Condition2 condition { get; set; }
        }

        public class Condition2
        {

        }
        public class SunriseSunsetData
        {
            public Results results { get; set; }
            public string status { get; set; }
        }

        public class Results
        {
            public string sunrise { get; set; }
            public string sunset { get; set; }
            public string solar_noon { get; set; }
            public string day_length { get; set; }
            public string civil_twilight_begin { get; set; }
            public string civil_twilight_end { get; set; }
            public string nautical_twilight_begin { get; set; }
            public string nautical_twilight_end { get; set; }
            public string astronomical_twilight_begin { get; set; }
            public string astronomical_twilight_end { get; set; }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        ////////////////////////////////////////////////// Functions /////////////////////////////////////////////////

        public void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        public static string ReplaceAllSpaces(string str)
        {
            return Regex.Replace(str, @"\s+", "%20");
        }

        public void InitializeCurrentWeatherData(string location)
        {
            string apiKeyAirQuality = "cac930095ee3fc5cbfeea4fc398454a66592e817"; // apiKey used for air quality data
            string apiKeyWeather = "4d9a0860b52d4ad185592147213011"; // apiKey used for main weather data
            string apiLoc = ReplaceAllSpaces(location); // Replaces whitespaces with %20 so the user can type: Los Angeles instead of Los-Angeles

            string airQualityString = $"https://api.waqi.info/feed/ {apiLoc}/?token= {apiKeyAirQuality}";
            string WeatherString = $"https://api.weatherapi.com/v1/forecast.json?key= {apiKeyWeather}&q={apiLoc}&days=5&aqi=no&alerts=no";
            WeatherString = Regex.Replace(WeatherString, @"\s+", "");
            airQualityString = Regex.Replace(airQualityString, @"\s+", "");

            try
            {
                using (var webClient = new WebClient())
                {
                    // Get Air Quality Data

                    var jsonAir = webClient.DownloadString(airQualityString);
                    JObject jsonResponse = JObject.Parse(jsonAir);
                    int aqi = (int)jsonResponse["data"]["aqi"];


                    // Load Air Quality Data

                    AqiText.Content = "Air Quality";
                    Aqi.Content = $"{aqi}";

                    if (aqi <= 40)
                    {
                        Aqi.Background = Brushes.Green;
                        //AqiStatus.Content = "Air quality is good"; // <-- Not needed but maybe will use someday 
                    }
                    else if (aqi > 40 && aqi <= 60)
                    {
                        Aqi.Background = Brushes.Orange;
                        //AqiStatus.Content = "Air quality could be better"; // <-- Not needed but maybe will use someday 
                    }
                    else if (aqi > 60 && aqi <= 90)
                    {
                        Aqi.Background = Brushes.Red;
                        //AqiStatus.Content = "Air quality is bad"; // <-- Not needed but maybe will use someday 
                    }
                    else
                    {
                        Aqi.Background = Brushes.Purple;
                        //AqiStatus.Content = "Air quality is very bad"; // <-- Not needed but maybe will use someday 
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not find air quality data for given city.");
            }


            try
            {
                using (var webClient = new WebClient())
                {
                    // Get Weather Data

                    var json = webClient.DownloadString(WeatherString);
                    WeatherData Weather = JsonConvert.DeserializeObject<WeatherData>(json);


                    // Get sunrise and sunset data 

                    string sunriseSunsetString = $"https://api.sunrise-sunset.org/json?lat= {Weather.location.lat}&lng= {Weather.location.lon}";
                    sunriseSunsetString = Regex.Replace(sunriseSunsetString, @"\s+", "");
                    var jsonSunrise = webClient.DownloadString(sunriseSunsetString);

                    SunriseSunsetData SunriseSunset = JsonConvert.DeserializeObject<SunriseSunsetData>(jsonSunrise);


                    // Compare sunset, sunrise and current time to know if the sun is still up

                    string date = Weather.location.localtime.Substring(0, 10);
                    string sunset = $"{date} {SunriseSunset.results.sunset}";
                    string sunrise = $"{date} {SunriseSunset.results.sunrise}";

                    //MessageBox.Show($"{Weather.location.localtime}\n{sunset}"); // <-- Keep this for troubleshooting DateTime errors

                    var currentTime = DateTime.ParseExact(Weather.location.localtime, "yyyy-MM-dd H:m", CultureInfo.InvariantCulture).TimeOfDay;
                    var sunsetTime = DateTime.ParseExact(sunset, "yyyy-MM-dd h:m:s tt", CultureInfo.InvariantCulture).TimeOfDay;
                    var sunriseTime = DateTime.ParseExact(sunrise, "yyyy-MM-dd h:m:s tt", CultureInfo.InvariantCulture).TimeOfDay;

                    bool sunStillUp = (currentTime < sunsetTime) && (currentTime > sunriseTime);


                    // If sun is not up, there's a need to load different icons (those that don't contain sun and are dark),
                    // These icons' names are ending with "n"

                    string resourcePostfix = "";

                    if (sunStillUp == false)
                        resourcePostfix = "n";

                    InitializeWeatherIcon(WeatherIcon, Weather, resourcePostfix, Weather.current.condition.code);


                    // Load Rectangles
                    MainRect.Opacity = 1;
                    Day1Rect.Opacity = 1;
                    Day2Rect.Opacity = 1;
                    Day3Rect.Opacity = 1;

                    // Load the weather status
                    WeatherStatus.Content = Weather.current.condition.text;

                    // Load Temperature Icon
                    TemperatureIcon.Source = (ImageSource)FindResource("thermometer");

                    // Load Temperature
                    Temperatue.Content = $"{Weather.current.temp_c} °C";

                    // Load FeelsLike temperature
                    FeelsLike.Content = $"Feels like {Weather.current.feelslike_c} °C";

                    // Load Wind Icon
                    WindImage.Source = (ImageSource)FindResource("wind");

                    // Load Wind Speed
                    WindSpeed.Content = $"{Weather.current.wind_kph} kph";

                    // Load Wind Direction

                    if (Weather.current.wind_dir == "W")
                        WindDirection.Content = "West";
                    else if (Weather.current.wind_dir == "E")
                        WindDirection.Content = "East";
                    else if (Weather.current.wind_dir == "N")
                        WindDirection.Content = "North";
                    else if (Weather.current.wind_dir == "S")
                        WindDirection.Content = "South";
                    else if (Weather.current.wind_dir == "NW")
                        WindDirection.Content = "North-West";
                    else if (Weather.current.wind_dir == "NE")
                        WindDirection.Content = "North-East";
                    else if (Weather.current.wind_dir == "SW")
                        WindDirection.Content = "South-West";
                    else
                        WindDirection.Content = "South-East";

                    // Load Pressure Icon
                    PressureIcon.Source = (ImageSource)FindResource("pressure_gauge");

                    // Load Pressure Data
                    Pressure.Content = $"{Weather.current.pressure_mb} hPa";

                    // Load Humidity Icon
                    HumidityIcon.Source = (ImageSource)FindResource("humidity_icon");

                    // Load Humdity Data
                    Humidity.Content = $"{Weather.current.humidity}%";

                    // Load Time Icon
                    TimeIcon.Source = (ImageSource)FindResource("clock");

                    // Load Time 
                    Weather.location.localtime = Weather.location.localtime.Remove(0, 11);
                    Time.Content = Weather.location.localtime;

                    // Load Location Icon
                    LocationIcon.Source = (ImageSource)FindResource("pin");

                    // Load Location Data
                    LocationData.Content = location;


                    // Initialize Forecast Data
                    InitializeForecastData(Weather);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Could not find weather data for given city.");
                //MessageBox.Show($"{er}"); // <-- Keep this for troubleshooting purposes
            }
        }

        public void InitializeWeatherIcon(Image WeatherIcon, WeatherData Weather, string resourcePostfix, int code)
        {
            // The switch statement determines what images should be displayed based on the weather code from weatherApi
            // There really is no elegant way to do this other than this, thank god for the switch statement

            switch (code)
            {
                case 1000:
                    // If the weather status is "Sunny", then for some reason the image is a moon, it's probably an api error
                    // and it needed manual fixing (if-else)

                    if (resourcePostfix == "n" && Weather.current.condition.text == "Sunny")
                        WeatherIcon.Source = (ImageSource)FindResource($"113");
                    else
                        WeatherIcon.Source = (ImageSource)FindResource($"113{resourcePostfix}");
                    break;
                case 1003:
                    WeatherIcon.Source = (ImageSource)FindResource($"116{resourcePostfix}");
                    break;
                case 1006:
                    WeatherIcon.Source = (ImageSource)FindResource($"119{resourcePostfix}");
                    break;
                case 1009:
                    WeatherIcon.Source = (ImageSource)FindResource($"122{resourcePostfix}");
                    break;
                case 1030:
                    WeatherIcon.Source = (ImageSource)FindResource($"143{resourcePostfix}");
                    break;
                case 1063:
                    WeatherIcon.Source = (ImageSource)FindResource($"176{resourcePostfix}");
                    break;
                case 1066:
                    WeatherIcon.Source = (ImageSource)FindResource($"179{resourcePostfix}");
                    break;
                case 1069:
                    WeatherIcon.Source = (ImageSource)FindResource($"182{resourcePostfix}");
                    break;
                case 1072:
                    WeatherIcon.Source = (ImageSource)FindResource($"185{resourcePostfix}");
                    break;
                case 1087:
                    WeatherIcon.Source = (ImageSource)FindResource($"200{resourcePostfix}");
                    break;
                case 1114:
                    WeatherIcon.Source = (ImageSource)FindResource($"227{resourcePostfix}");
                    break;
                case 1117:
                    WeatherIcon.Source = (ImageSource)FindResource($"230{resourcePostfix}");
                    break;
                case 2485:
                    WeatherIcon.Source = (ImageSource)FindResource($"260{resourcePostfix}");
                    break;
                case 1147:
                    WeatherIcon.Source = (ImageSource)FindResource($"263{resourcePostfix}");
                    break;
                case 1150:
                    WeatherIcon.Source = (ImageSource)FindResource($"266{resourcePostfix}");
                    break;
                case 1153:
                    WeatherIcon.Source = (ImageSource)FindResource($"281{resourcePostfix}");
                    break;
                case 1168:
                    WeatherIcon.Source = (ImageSource)FindResource($"284{resourcePostfix}");
                    break;
                case 1171:
                    WeatherIcon.Source = (ImageSource)FindResource($"293{resourcePostfix}");
                    break;
                case 1180:
                    WeatherIcon.Source = (ImageSource)FindResource($"296{resourcePostfix}");
                    break;
                case 1183:
                    WeatherIcon.Source = (ImageSource)FindResource($"299{resourcePostfix}");
                    break;
                case 1186:
                    WeatherIcon.Source = (ImageSource)FindResource($"302{resourcePostfix}");
                    break;
                case 1189:
                    WeatherIcon.Source = (ImageSource)FindResource($"305{resourcePostfix}");
                    break;
                case 1192:
                    WeatherIcon.Source = (ImageSource)FindResource($"308{resourcePostfix}");
                    break;
                case 1195:
                    WeatherIcon.Source = (ImageSource)FindResource($"311{resourcePostfix}");
                    break;
                case 1198:
                    WeatherIcon.Source = (ImageSource)FindResource($"314{resourcePostfix}");
                    break;
                case 1201:
                    WeatherIcon.Source = (ImageSource)FindResource($"317{resourcePostfix}");
                    break;
                case 1204:
                    WeatherIcon.Source = (ImageSource)FindResource($"320{resourcePostfix}");
                    break;
                case 1207:
                    WeatherIcon.Source = (ImageSource)FindResource($"323{resourcePostfix}");
                    break;
                case 1210:
                    WeatherIcon.Source = (ImageSource)FindResource($"326{resourcePostfix}");
                    break;
                case 1213:
                    WeatherIcon.Source = (ImageSource)FindResource($"329{resourcePostfix}");
                    break;
                case 1216:
                    WeatherIcon.Source = (ImageSource)FindResource($"332{resourcePostfix}");
                    break;
                case 1219:
                    WeatherIcon.Source = (ImageSource)FindResource($"335{resourcePostfix}");
                    break;
                case 1222:
                    WeatherIcon.Source = (ImageSource)FindResource($"338{resourcePostfix}");
                    break;
                case 1225:
                    WeatherIcon.Source = (ImageSource)FindResource($"350{resourcePostfix}");
                    break;
                case 1237:
                    WeatherIcon.Source = (ImageSource)FindResource($"353{resourcePostfix}");
                    break;
                case 1240:
                    WeatherIcon.Source = (ImageSource)FindResource($"356{resourcePostfix}");
                    break;
                case 1243:
                    WeatherIcon.Source = (ImageSource)FindResource($"359{resourcePostfix}");
                    break;
                case 1246:
                    WeatherIcon.Source = (ImageSource)FindResource($"362{resourcePostfix}");
                    break;
                case 1249:
                    WeatherIcon.Source = (ImageSource)FindResource($"365{resourcePostfix}");
                    break;
                case 1252:
                    WeatherIcon.Source = (ImageSource)FindResource($"368{resourcePostfix}");
                    break;
                case 1255:
                    WeatherIcon.Source = (ImageSource)FindResource($"371{resourcePostfix}");
                    break;
                case 1258:
                    WeatherIcon.Source = (ImageSource)FindResource($"374{resourcePostfix}");
                    break;
                case 1261:
                    WeatherIcon.Source = (ImageSource)FindResource($"377{resourcePostfix}");
                    break;
                case 1264:
                    WeatherIcon.Source = (ImageSource)FindResource($"386{resourcePostfix}");
                    break;
                case 1273:
                    WeatherIcon.Source = (ImageSource)FindResource($"389{resourcePostfix}");
                    break;
                case 1276:
                    WeatherIcon.Source = (ImageSource)FindResource($"392{resourcePostfix}");
                    break;
                case 1279:
                    WeatherIcon.Source = (ImageSource)FindResource($"395{resourcePostfix}");
                    break;
            }
        }

        public void InitializeWeatherDataBasedOnUserLocation()
        {
            // Call this function at start of the app 
            string city = GetUserCity();
            InitializeCurrentWeatherData(city);
        }

        public static string GetIPAddress()
        {
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString("https://api.ipify.org/?format=json");
                IPData IP = JsonConvert.DeserializeObject<IPData>(json);

                return IP.ip;
            }
        }

        public static string GetUserCity()        
        {
            string apiKey = "3068adc9b24a344b007984a921b5cacef40aab05b221c4e9ea85bb02";
            string URL = $"https://api.ipdata.co/ {GetIPAddress()}?api-key= {apiKey}";
            URL = Regex.Replace(URL, @"\s+", "");


            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(URL);
                JObject jsonResponse = JObject.Parse(json);

                return (string)jsonResponse["city"];
            }
        }

        public void InitializeForecastData(WeatherData Weather)
        {
            // Load Text

            ForecastText.Content = "Forecast";

            // Load icons for forecast

            InitializeWeatherIcon(WeatherIconDay1, Weather, "", Weather.forecast.forecastday[0].day.condition.code);
            InitializeWeatherIcon(WeatherIconDay2, Weather, "", Weather.forecast.forecastday[1].day.condition.code);
            InitializeWeatherIcon(WeatherIconDay3, Weather, "", Weather.forecast.forecastday[2].day.condition.code);

            // Load temperatures for forecast

            TemperatureDay1.Content = $"{Weather.forecast.forecastday[0].day.avgtemp_c} °C";
            TemperatureDay2.Content = $"{Weather.forecast.forecastday[1].day.avgtemp_c} °C";
            TemperatureDay3.Content = $"{Weather.forecast.forecastday[2].day.avgtemp_c} °C";

            // Load weather status for forecast

            WeatherStatusDay1.Content = Weather.forecast.forecastday[0].day.condition.text;
            WeatherStatusDay2.Content = Weather.forecast.forecastday[1].day.condition.text;
            WeatherStatusDay3.Content = Weather.forecast.forecastday[2].day.condition.text;

            // Load dates for forecast

            DateDay1.Content = "Today";
            DateDay2.Content = Weather.forecast.forecastday[1].date;
            DateDay3.Content = Weather.forecast.forecastday[2].date;

        }

        private void Load_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            // Loc.Text is equal to entered text in the textbox
            if (Loc.Text == "")
                MessageBox.Show("Location field is empty.");
            else
                InitializeCurrentWeatherData(Loc.Text);
        }

        private void MyLocation_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            InitializeWeatherDataBasedOnUserLocation();
        }
    }
}
