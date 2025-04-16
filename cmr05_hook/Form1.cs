using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;

namespace cmr05_hook
{
    public partial class Form1 : Form
    {
        const string game_exe = "CMR5";
        Process game;

        static byte[] asm_reset = {0xE8, 0xA5, 0xC3, 0xFF, 0xFF};
        static byte[] asm_reset_nop = {0x90, 0x90, 0x90, 0x90, 0x90};
        const uint adrr_reset = 0x00494916;
        const uint adrr_reset2 = 0x00496626;

        KeyHook.GlobalKeyboardHook gkh;
        MemoryEdit.Memory mem;
        SoundPlayer snd;

        public Form1()
        {
            InitializeComponent();
            mem = new MemoryEdit.Memory();
            gkh = new KeyHook.GlobalKeyboardHook();
            gkh.Hook();
            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            snd = new SoundPlayer();
        }

        private void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (!mem.IsFocused()) return;
            byte[] code;
            if (e.KeyCode == Keys.F12)
            {
                byte[] tmp = mem.ReadBytes(adrr_reset, 5);
                if (tmp.SequenceEqual(asm_reset))
                {
                    code = asm_reset_nop;
                    snd.Stream = Properties.Resources.eng;
                }
                else
                {
                    code = asm_reset;
                    snd.Stream = Properties.Resources.dis;
                }
                snd.Play();
                mem.WriteBytes(adrr_reset, code, 5);
                mem.WriteBytes(adrr_reset2, code, 5);
            }
        }

        private void ScanForGame()
        {
            Process[] procs = Process.GetProcessesByName(game_exe);
            if (procs.Length > 0)
            {
                game = procs[0];
                mem.Attach((uint)game.Id, MemoryEdit.Memory.ProcessAccessFlags.All);
            }
        }

        private void tmr_scan_Tick(object sender, EventArgs e)
        {
            if (game == null || game.HasExited) ScanForGame();
        }
    }
}