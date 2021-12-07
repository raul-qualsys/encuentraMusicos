using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace encuentraMusicos.Classes
{
    public interface ISQLiteDB
    {
        SQLiteAsyncConnection GetConnection();
    }
}
