namespace Nzr.Orm.Core
{
    /// <summary>
    /// The supported naming styles used to retrieve table and column names
    /// based on entity type and properties.
    /// </summary>
    public enum NamingStyle
    {
        /// <summary>
        /// my_table, my_columm
        /// </summary>
        LowerCaseUnderlined = 0,

        /// <summary>
        /// mytable, mycolumn
        /// </summary>
        LowerCase = 1,

        /// <summary>
        /// My_Table, My_Column
        /// </summary>
        PascalCaseUnderlined = 2,

        /// <summary>
        /// MyTable, MyColumn
        /// </summary>
        PascalCase = 3
    }
}