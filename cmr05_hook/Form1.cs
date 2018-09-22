using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace cmr05_hook
{
    public partial class Form1 : Form
    {
        KeyHook.GlobalKeyboardHook gkh;
        MemoryEdit.Memory mem;
        SoundPlayer snd;

        long opcode = 0xFFFFC3A5E8;
        byte[] code;
        const long NOP = 0x9090909090;

        public Form1()
        {
            InitializeComponent();
            gkh = new KeyHook.GlobalKeyboardHook();
            gkh.Hook();
            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            snd = new SoundPlayer();
        }

        private void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                if (mem == null)
                {
                    if (MemoryEdit.Memory.IsProcessOpen("CMR5"))
                        mem = new MemoryEdit.Memory("CMR5", 0x001F0FFF);
                }
                if (mem.ReadByte(0x00494916, 1) == 0x90)
                {
                    code = BitConverter.GetBytes(opcode);
                    snd.Stream = Properties.Resources.dis;
                }
                else
                {
                    code = BitConverter.GetBytes(NOP);
                    snd.Stream = Properties.Resources.eng;
                }
                snd.Play();
                mem.WriteByte(0x00494916, code, 5);
                mem.WriteByte(0x00496626, code, 5);
            }
        }
    }
}