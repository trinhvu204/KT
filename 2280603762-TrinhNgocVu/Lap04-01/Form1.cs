using Lap04_01.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lap04_01
{
    public partial class Form1 : Form
    {
        StudentContextDB contextDB = new StudentContextDB();
        private int totalStudents = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                try
            {
                List<Faculty> faculties = contextDB.Faculties.ToList();
                List<Student> students= contextDB.Students.ToList();
                FillFacultycombobox(faculties);
                BinhGrid(students);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            rbnNu.Checked = true;
            txtTongNu.Text = totalStudents.ToString();
            txtTongNu.Enabled = false;
            txtTongNam.Text = totalStudents.ToString();
            txtTongNam.Enabled = false;
        }
        private void FillFacultycombobox(List<Faculty> faculties) { 
            this.cmbKhoa.DataSource = faculties;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }
        private void BinhGrid(List<Student> students) {
            dvgStudent.Rows.Clear();
            foreach (var item in students) {
                int index = dvgStudent.Rows.Add();
                dvgStudent.Rows[index].Cells[0].Value = item.StudentID;
                dvgStudent.Rows[index].Cells[1].Value = item.StudentName;
                dvgStudent.Rows[index].Cells[2].Value = item.Gender;
                dvgStudent.Rows[index].Cells[3].Value = item.AverageScore;
                dvgStudent.Rows[index].Cells[4].Value = item.FacultyID;
            }
        }

        private void dvgStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectRow = dvgStudent.Rows [e.RowIndex];
                txtMSSV.Text = selectRow.Cells[0].Value.ToString();
                txtHOTEN.Text = selectRow.Cells[1].Value.ToString();
                string gender  = selectRow.Cells[2].Value.ToString();
                if(gender == "Male")
                    rbnNam.Checked = true;
                else
                    rbnNu .Checked = true;
                txtDiem.Text = selectRow.Cells[3].Value.ToString();
                cmbKhoa.Text =  selectRow.Cells[4].Value.ToString();
            }
            
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Ban chac chan thoat khong","Thong bao",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if( result == DialogResult.Yes)
                this.Close();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||string.IsNullOrWhiteSpace(txtHOTEN.Text) ||
               string.IsNullOrWhiteSpace(txtDiem.Text))
            {
                MessageBox.Show("Vui Long phap du thong tin", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    List<Student> students = contextDB.Students.ToList();
                    if(students.Any(s => s.StudentID == txtMSSV.Text))
                    {
                        MessageBox.Show("Ma SV da ton tai", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!Regex.IsMatch(txtMSSV.Text, @"^\d{10}$"))
                    {
                        MessageBox.Show("Ma khong hop le 10 so nha.", "Thong Bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!Regex.IsMatch(txtHOTEN.Text, @"^[a-zA-Z\s]{3,100}$"))
                    {
                        MessageBox.Show("Tên sinh viên không hợp lệ. Tên chỉ chứa chữ cái và có độ dài từ 3 đến 100 ký tự.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    decimal score;
                    if (!decimal.TryParse(txtDiem.Text, out score) || score < 0 || score > 10)
                    {
                        MessageBox.Show("Điểm trung bình không hợp lệ. Điểm phải là số từ 0 đến 10.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var newStudent = new Student
                    {
                        StudentID = txtMSSV.Text,
                        StudentName = txtHOTEN.Text,
                        Gender = rbnNam.Checked ? "Male" : "Female",
                        AverageScore = int.Parse(txtDiem.Text),
                        FacultyID = int.Parse(cmbKhoa.SelectedValue.ToString())
                        
                    };
                   
                    contextDB.Students.Add(newStudent);
                    contextDB.SaveChanges();
                    BinhGrid(contextDB.Students.ToList());
                    FillFacultycombobox(contextDB.Faculties.ToList());
                    totalStudents = contextDB.Students.Count();
                   

                    MessageBox.Show("Them sinh vien thanh cong","Thong bao",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Loi phap du lieu: {ex.Message}","Thong bao",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                List<Student> students = contextDB.Students.ToList();
                
                var student = students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (student != null)
                {
                    if (students.Any(s => s.StudentID == txtMSSV.Text && s.StudentID != student.StudentID))
                        {
                        MessageBox.Show("Ma SV da ton tai", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    student.StudentName = txtHOTEN.Text;
                    student.Gender = rbnNam.Checked ? "Male" : "Female";
                    student.AverageScore = int.Parse(txtDiem.Text);
                    student.FacultyID = int.Parse(cmbKhoa.SelectedValue.ToString());
                    contextDB.SaveChanges();

                    BinhGrid(contextDB.Students.ToList());
                    MessageBox.Show("tHEM THANH CONG", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi phap du lieu: {ex.Message}", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                List<Student> students = contextDB.Students.ToList();
                var student = students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (student != null)
                {
                    contextDB.Students.Remove(student);
                    contextDB.SaveChanges();
                    BinhGrid(contextDB.Students.ToList());
                    MessageBox.Show("Xoa thanh cong","Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Xin vien khong tim thay", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) {
                MessageBox.Show($"Loi phap du lieu: {ex.Message}", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
