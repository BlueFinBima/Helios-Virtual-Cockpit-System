﻿//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
	using System.Globalization;
	using System.Windows;
    using System.Windows.Media;
	using System.Xml;

	[HeliosControl("Helios.Base.CustomDrum", "Custom Drum", "Miscellaneous", typeof(Gauges.GaugeRenderer))]

    public class CustomDrum : Gauges.BaseGauge
	{

        private HeliosValue _drumOffset;



		private Gauges.CustomGaugeNeedle _Drum;
		private string _drumImage = "{Helios}/Gauges/A-10/Common/drum_tape.xaml";
		private double _gaugeWidth = 50d;
		private double _gaugeHeight = 100d;
		private double _drum_PosX = 0d;
		private double _drum_PosY = 0d;
		private double _drum_Width = 50d;
		private double _drum_Height = 1000d;
		private double _drum_CenterX = 0d;
		private double _drum_CenterY = 0d;
		private int _initialVertical = 0;
		private int _minVertical = 0;
		private int _verticalTravel = 0;
		private double _minInputVertical = 0d;
		private double _maxInputVertical = 1d;
		


		public CustomDrum()
            : base("CustomDrum", new Size(50d, 100d))
        {
	

			_Drum = new Gauges.CustomGaugeNeedle(_drumImage, new Point(0, 0), new Size(50, 1000), new Point(0, 0));
            _Drum.Clip = new RectangleGeometry(new Rect(1d, 1d,50d, 100d));
            Components.Add(_Drum);

            _drumOffset = new HeliosValue(this, new BindingValue(0d), "", "Drum vertical offset", "Values between configured Min and Max", "", BindingValueUnits.Numeric);
            _drumOffset.Execute += new HeliosActionHandler(DrumOffset_Execute);
            Actions.Add(_drumOffset);
            Values.Add(_drumOffset);

		}


		#region Properties

		public string DrumImage
		{
			get
			{
				return _drumImage;
			}
			set
			{
				if ((_drumImage == null && value != null)
					|| (_Drum != null && !_drumImage.Equals(value)))
				{
					string oldValue = _drumImage;
					_drumImage = value;
					OnPropertyChanged("DrumImage", oldValue, value, true);
					_Drum.Image = _drumImage;
					Refresh();
				}
			}
		}



		public double Drum_PosX
		{
			get
			{
				return _drum_PosX;
			}
			set
			{
				if (!_drum_PosX.Equals(value))
				{
					double oldValue = _drum_PosX;
					_drum_PosX = value;
					OnPropertyChanged("Drum_PosX", oldValue, value, true);
					_Drum.TapePosX = _drum_PosX;
					Refresh();
				}
			}
		}

		public double Drum_PosY
		{
			get
			{
				return _drum_PosY;
			}
			set
			{
				if (!_drum_PosY.Equals(value))
				{
					double oldValue = _drum_PosY;
					_drum_PosY = value;
					OnPropertyChanged("Drum_PosY", oldValue, value, true);
					_Drum.TapePosY = _drum_PosY;
					Refresh();
				}
			}
		}


		public double Drum_Width
		{
			get
			{
				return _drum_Width;
			}
			set
			{
				if (!_drum_Width.Equals(value))
				{
					double oldValue = _drum_Width;
					_drum_Width = value;
					OnPropertyChanged("Drum_Width", oldValue, value, true);
					_Drum.Tape_Width = _drum_Width;
					Refresh();
				}
			}
		}

		public double Drum_Height
		{
			get
			{
				return _drum_Height;
			}
			set
			{
				if (!_drum_Height.Equals(value))
				{
					double oldValue = _drum_Height;
					_drum_Height = value;
					OnPropertyChanged("Drum_Height", oldValue, value, true);
					_Drum.Tape_Height = _drum_Height;
					Refresh();
				}
			}
		}



		public int InitialVertical
		{
			get
			{
				return _initialVertical;
			}
			set
			{
				if (!_initialVertical.Equals(value))
				{
					int oldValue = _initialVertical;
					_initialVertical = value;
					OnPropertyChanged("InitialVertical", oldValue, value, true);
					_Drum.VerticalOffset = value;
					Refresh();
				}
			}
		}

		public int MinVertical
		{
			get
			{
				return _minVertical;
			}
			set
			{
				if (!_minVertical.Equals(value))
				{
					int oldValue = _minVertical;
					_minVertical = value;
					OnPropertyChanged("MinVertical", oldValue, value, true);
					_Drum.VerticalOffset = value;
					Refresh();
				}
			}
		}

		public int VerticalTravel
		{
			get
			{
				return _verticalTravel;
			}
			set
			{
				if (!_verticalTravel.Equals(value))
				{
					int oldValue = _verticalTravel;
					_verticalTravel = value;
					OnPropertyChanged("VerticalTravel", oldValue, value, true);
					_Drum.VerticalOffset = value;
					Refresh();
				}
			}
		}



		public double MinInputVertical
		{
			get
			{
				return _minInputVertical;
			}
			set
			{
				if (!_minInputVertical.Equals(value))
				{
					double oldValue = _minInputVertical;
					_minInputVertical = value;
					OnPropertyChanged("MinInputVertical", oldValue, value, true);
				}
			}
		}

		public double MaxInputVertical
		{
			get
			{
				return _maxInputVertical;
			}
			set
			{
				if (!_maxInputVertical.Equals(value))
				{
					double oldValue = _maxInputVertical;
					_maxInputVertical = value;
					OnPropertyChanged("MaxInputVertical", oldValue, value, true);
				}
			}
		}


		public override void Reset()
		{
			BeginTriggerBypass(true);
				_Drum.VerticalOffset = InitialVertical;
				Refresh();
			EndTriggerBypass(true);
		}



		#endregion


		#region Actions

		
        void DrumOffset_Execute(object action, HeliosActionEventArgs e)
        {
            _drumOffset.SetValue(e.Value, e.BypassCascadingTriggers);
			double vValue = (_drumOffset.Value.DoubleValue + ((_maxInputVertical - _minInputVertical) - _maxInputVertical)) / (_maxInputVertical - _minInputVertical); // convert to to 0-1
			_Drum.VerticalOffset = (_minVertical * (1 - vValue)) + (_verticalTravel * vValue); // lerp vertical 
		}

	

		#endregion

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);

			writer.WriteElementString("DrumImage", DrumImage);
			writer.WriteElementString("Drum_PosX", Drum_PosX.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_PosY", Drum_PosY.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_Width", Drum_Width.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Drum_Height", Drum_Height.ToString(CultureInfo.InvariantCulture));		
			writer.WriteElementString("MinVertical", MinVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("VerticalTravel", VerticalTravel.ToString(CultureInfo.InvariantCulture));			
			writer.WriteElementString("InitialVertical", InitialVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MinInputVertical", MinInputVertical.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("MaxInputVertical", MaxInputVertical.ToString(CultureInfo.InvariantCulture));

		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			DrumImage = reader.ReadElementString("DrumImage");
			Drum_PosX = double.Parse(reader.ReadElementString("Drum_PosX"), CultureInfo.InvariantCulture);
			Drum_PosY = double.Parse(reader.ReadElementString("Drum_PosY"), CultureInfo.InvariantCulture);
			Drum_Width = double.Parse(reader.ReadElementString("Drum_Width"), CultureInfo.InvariantCulture);
			Drum_Height = double.Parse(reader.ReadElementString("Drum_Height"), CultureInfo.InvariantCulture);			
			MinVertical = int.Parse(reader.ReadElementString("MinVertical"), CultureInfo.InvariantCulture);
			VerticalTravel = int.Parse(reader.ReadElementString("VerticalTravel"), CultureInfo.InvariantCulture);			
			InitialVertical = int.Parse(reader.ReadElementString("InitialVertical"), CultureInfo.InvariantCulture);
			MinInputVertical = double.Parse(reader.ReadElementString("MinInputVertical"), CultureInfo.InvariantCulture);
			MaxInputVertical = double.Parse(reader.ReadElementString("MaxInputVertical"), CultureInfo.InvariantCulture);

			BeginTriggerBypass(true);
				_Drum.VerticalOffset = InitialVertical;
				Refresh();
			EndTriggerBypass(true);
		}



	}
}
