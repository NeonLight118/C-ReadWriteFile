using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace raFile1
{

    class AccountRecordRA : AccountRecord
    {
        public AccountRecordRA()
            : base()
        {
            // no arg parent (base) constructor called
        }
        public AccountRecordRA(int acct, string fn, string ln, double bal)
            : base(acct, fn, ln, bal)
        {
            // 4 arg parent constructor called 
        }
        public void write(FileStream raFile)
        {
            BinaryWriter bw = new BinaryWriter(raFile);
            // Write record data in field order
            bw.Write(Account);
            formatName(bw, FirstName);
            formatName(bw, LastName);
            bw.Write(Balance);
        }
        private void formatName(BinaryWriter bw, string n)
        {
            StringBuilder sb = new StringBuilder(n);
            sb.Length = 15;
            bw.Write(sb.ToString());
        }
        public void read(FileStream raFile)
        {
            BinaryReader br = new BinaryReader(raFile);
            Account = br.ReadInt32();
            FirstName = br.ReadString().Replace('\0', ' ');
            LastName = br.ReadString().Replace('\0', ' ');
            Balance = br.ReadDouble();
        }
    }
}
