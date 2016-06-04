/* MarcRelatedRecord.cs -- 
 * Ars Magna project, http://arsmagna.ru
 */

#region

using JetBrains.Annotations;

#endregion

namespace ManagedClient.Marc
{
    /// <summary>
    /// Необходимость связанной записи.
    /// </summary>
    [PublicAPI]
    public enum MarcRelatedRecord
        : byte
    {
        /// <summary>
        /// Не требуется.
        /// </summary>
        NotRequired = (byte)' ',

        /// <summary>
        /// Требуется.
        /// </summary>
        Required = (byte)'r'
    }
}
