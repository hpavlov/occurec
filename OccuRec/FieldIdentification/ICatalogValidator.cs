using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.FieldIdentification
{
	public enum StarCatalog
	{
		NotSpecified = 0,
		UCAC2 = 1,
		NOMAD = 2,
		UCAC3 = 3,
		PPMXL = 4,
		UCAC4 = 5
	}

	public enum MagOutputBand
	{
		JohnsonV,
		CousinsR,
		//Instrumental
	}

	public enum MagInputBand
	{
		Unfiltered,
		JohnsonV,
		CousinsR,
		SLOAN_r,
		SLOAN_g
	}

	public class CatalogMagnitudeBand
	{
		private string m_DisplayName;
		private Guid m_Id;

		public Guid Id
		{
			get { return m_Id; }
		}

		public CatalogMagnitudeBand(Guid id, string displayName)
		{
			m_DisplayName = displayName;
			m_Id = id;
		}

		public override string ToString()
		{
			return m_DisplayName;
		}
	}

	public interface ICatalogValidator
	{
		bool IsValidCatalogLocation(StarCatalog catalog, ref string path);
		bool VerifyCurrentCatalogue(StarCatalog catalog, ref string path);
		object[] MagnitudeBandsForCatalog(StarCatalog catalog);
	}
}
