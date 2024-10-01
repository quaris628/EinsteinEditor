using Einstein.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.config.bibiteVersions.vanilla
{
    public abstract class BibiteVanillaVersion : BibiteVersion, IComparable<BibiteVanillaVersion>
    {
        // used to determine the sequential order of each version
        private int vanillaVersionId;

        protected BibiteVanillaVersion(int vanillaVersionId)
        {
            this.vanillaVersionId = vanillaVersionId;
        }

        public override BibiteVanillaVersion GetVanilla()
        {
            return this;
        }

        protected abstract BaseBrain CreateVersionUpCopyOf(BaseBrain brain);

        protected abstract BaseBrain CreateVersionDownCopyOf(BaseBrain brain);

        internal static BaseBrain CreateConvertedCopyOf(BaseBrain vanillabrain, BibiteVanillaVersion targetVersion)
        {
            if (!(vanillabrain.BibiteVersion is BibiteVanillaVersion))
            {
                throw new InvalidOperationException("vanillabrain must be vanilla");
            }
            else if (vanillabrain.BibiteVersion.GetVanilla() == targetVersion)
            {
                return vanillabrain;
            }
            else if (vanillabrain.BibiteVersion.GetVanilla() < targetVersion)
            {
                // convert upwards
                do
                {
                    vanillabrain = vanillabrain.BibiteVersion.GetVanilla().CreateVersionUpCopyOf(vanillabrain);
                }
                while (vanillabrain.BibiteVersion.GetVanilla() < targetVersion);
                return vanillabrain;
            }
            else // if (targetVersion < vanillabrain.BibiteVersion.GetVanilla())
            {
                do
                {
                    vanillabrain = vanillabrain.BibiteVersion.GetVanilla().CreateVersionDownCopyOf(vanillabrain);
                }
                while (vanillabrain.BibiteVersion.GetVanilla() > targetVersion);
                return vanillabrain;
            }
        }

        #region Generic Overrides

        public int CompareTo(BibiteVanillaVersion other)
        {
            return vanillaVersionId.CompareTo(other.vanillaVersionId);
        }

        public static bool operator >(BibiteVanillaVersion v1, BibiteVanillaVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(BibiteVanillaVersion v1, BibiteVanillaVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >=(BibiteVanillaVersion v1, BibiteVanillaVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public static bool operator <=(BibiteVanillaVersion v1, BibiteVanillaVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        #endregion Generic Overrides
    }
}
