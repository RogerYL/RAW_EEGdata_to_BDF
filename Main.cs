private void btn_SaveData_Click(object sender, EventArgs e)
        {
            if(_newdatarecorded == false)//if (rawdatalst == null)
            {
                MessageBox.Show("error");
                return;
            }

            _newdatarecorded = false;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "EPROM数据文件(*.edf)|*.edf|BioSemi数据文件(*.bdf)|*.bdf";
            dlg.Title = "保存数据文件";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            if (DialogResult.OK != dlg.ShowDialog())
                return;
            File_Generator Generat_file = new File_Generator();
            Generat_file.SaveSignal(dlg.FileName);
        }
