namespace Isolib.STFSPackage
{
    /// <summary>Contains function/s that deals with STFS License entries</summary>
    /// <seealso cref="HeaderData"/>
    public sealed class License
    {
        private readonly long _licenseID; //XUID, PUID or XBID of the license
        private readonly int _licenseBits; //Bits to determine the license type
        private readonly int _licenseFlags; //Flags to find out the license type

        /// <summary>Initiate the license entry constructor</summary>
        /// <param name="theID">The license id</param>
        /// <param name="theBits">The license bits</param>
        /// <param name="theFlags">The license flags</param>
        public License(long theID, int theBits, int theFlags)
        {
            _licenseBits = theBits;
            _licenseFlags = theFlags;
            _licenseID = theID;
        }

        #region properties
        /// <summary>Get the license flags</summary>
        public int LicenseFlags
        {
            get
            {
                return _licenseFlags;
            }
        }

        /// <summary>Get the license ID</summary>
        public long LicenseID
        {
            get
            {
                return _licenseID;
            }
        }

        /// <summary>Get the license bits</summary>
        public int LicenseBits
        {
            get
            {
                return _licenseBits;
            }
        }
        #endregion
    }
}

