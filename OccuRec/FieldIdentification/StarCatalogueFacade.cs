using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.FieldIdentification.NOMAD;
using OccuRec.FieldIdentification.PPMXL;
using OccuRec.FieldIdentification.UCAC2;
using OccuRec.FieldIdentification.UCAC3;
using OccuRec.FieldIdentification.UCAC4;
using OccuRec.Properties;

namespace OccuRec.FieldIdentification
{
	public class StarCatalogueFacade : ICatalogValidator
	{
		public string CatalogNETCode
		{
			get
			{
				return Settings.Default.StarCatalog.ToString();
			}
		}

		public CatalogMagnitudeBand[] CatalogMagnitudeBands
		{
			get
			{
				if (Settings.Default.StarCatalog == StarCatalog.UCAC2)
				{
					return UCAC2Catalogue.CatalogMagnitudeBands;
				}
				else if (Settings.Default.StarCatalog == StarCatalog.UCAC3)
				{
					return UCAC3Catalogue.CatalogMagnitudeBands;
				}
				else if (Settings.Default.StarCatalog == StarCatalog.NOMAD)
				{
					return NOMADCatalogue.CatalogMagnitudeBands;
				}
				else if (Settings.Default.StarCatalog == StarCatalog.PPMXL)
				{
					return PPMXLCatalogue.CatalogMagnitudeBands;
				}
				else if (Settings.Default.StarCatalog == StarCatalog.UCAC4)
				{
					return UCAC4Catalogue.CatalogMagnitudeBands;
				}

				return new CatalogMagnitudeBand[] { };
			}
		}

		public List<IStar> GetStarsInRegion(double raDeg, double deDeg, double diameterDeg, double limitMag, float epoch)
		{
			return GetStarsInRegion(Settings.Default.StarCatalog, Settings.Default.StarCatalogLocation, raDeg, deDeg, diameterDeg, limitMag, epoch);
		}

		public List<IStar> GetStarsInRegion(StarCatalog catalog, string catalogLocation, double raDeg, double deDeg, double diameterDeg, double limitMag, float epoch)
		{
			if (catalog == StarCatalog.UCAC2)
			{
				UCAC2Catalogue cat = new UCAC2Catalogue(catalogLocation);
				List<IStar> ucac2Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ucac2Stars;
			}
			else if (catalog == StarCatalog.UCAC3)
			{
				UCAC3Catalogue cat = new UCAC3Catalogue(catalogLocation);
				List<IStar> ucac3Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ucac3Stars;
			}
			else if (catalog == StarCatalog.NOMAD)
			{
				NOMADCatalogue cat = new NOMADCatalogue(catalogLocation);
				List<IStar> nomadStars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)nomadStars;
			}
			else if (catalog == StarCatalog.PPMXL)
			{
				PPMXLCatalogue cat = new PPMXLCatalogue(catalogLocation);
				List<IStar> ppmxlStars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ppmxlStars;
			}
			else if (catalog == StarCatalog.UCAC4)
			{
				UCAC4Catalogue cat = new UCAC4Catalogue(catalogLocation);
				List<IStar> ucac4Stars = cat.GetStarsInRegion(raDeg, deDeg, diameterDeg, limitMag, epoch);
				return (List<IStar>)ucac4Stars;
			}

			return null;
		}

		public double ConvertMagnitude(double measuredMag, double vrColorIndex, Guid catalogMagBand, MagOutputBand magOutputBand)
		{
			if (Settings.Default.StarCatalog == StarCatalog.UCAC2)
			{
				return UCAC2Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
			}
			else if (Settings.Default.StarCatalog == StarCatalog.UCAC3)
			{
				return UCAC3Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
			}
			else if (Settings.Default.StarCatalog == StarCatalog.NOMAD)
			{
				return NOMADCatalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
			}
			else if (Settings.Default.StarCatalog == StarCatalog.PPMXL)
			{
				return PPMXLCatalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
			}
			else if (Settings.Default.StarCatalog == StarCatalog.UCAC4)
			{
				return UCAC4Catalogue.ConvertMagnitude(measuredMag, vrColorIndex, catalogMagBand, magOutputBand);
			}

			return double.NaN;
		}

		public bool IsValidCatalogLocation(StarCatalog catalog, ref string folderPath)
		{
			if (catalog == StarCatalog.UCAC2)
				return UCAC2Catalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == StarCatalog.NOMAD)
				return NOMADCatalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == StarCatalog.UCAC3)
				return UCAC3Catalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == StarCatalog.PPMXL)
				return PPMXLCatalogue.IsValidCatalogLocation(ref folderPath);
			else if (catalog == StarCatalog.UCAC4)
				return UCAC4Catalogue.IsValidCatalogLocation(ref folderPath);

			return false;
		}

		public bool VerifyCurrentCatalogue(StarCatalog catalog, ref string path)
		{
			if (catalog == StarCatalog.UCAC2)
			{
				if (!UCAC2Catalogue.IsValidCatalogLocation(ref path))
					return false;

				if (!UCAC2Catalogue.CheckAndWarnIfNoBSS(path, null))
					return false;
			}
			else if (catalog == StarCatalog.NOMAD)
			{
				if (!NOMADCatalogue.IsValidCatalogLocation(ref path))
					return false;

				// TODO: Check index files??
			}
			else if (catalog == StarCatalog.UCAC3)
			{
				if (!UCAC3Catalogue.IsValidCatalogLocation(ref path))
					return false;

			}
			else if (catalog == StarCatalog.PPMXL)
			{
				if (!PPMXLCatalogue.IsValidCatalogLocation(ref path))
					return false;

			}
			else if (catalog == StarCatalog.UCAC4)
			{
				if (!UCAC4Catalogue.IsValidCatalogLocation(ref path))
					return false;

			}

			return true;
		}

		public object[] MagnitudeBandsForCatalog(StarCatalog catalog)
		{
			if (catalog == StarCatalog.UCAC2)
				return UCAC2Catalogue.CatalogMagnitudeBands;
			else if (catalog == StarCatalog.NOMAD)
				return NOMADCatalogue.CatalogMagnitudeBands;
			else if (catalog == StarCatalog.UCAC3)
				return UCAC3Catalogue.CatalogMagnitudeBands;
			else if (catalog == StarCatalog.PPMXL)
				return PPMXLCatalogue.CatalogMagnitudeBands;
			else if (catalog == StarCatalog.UCAC4)
				return UCAC4Catalogue.CatalogMagnitudeBands;

			return new object[] { };
		}

	}
}
