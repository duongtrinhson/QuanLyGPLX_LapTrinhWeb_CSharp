﻿using QuanLyGPLX_LapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace QuanLyGPLX_LapTrinhWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        MyDataDataContext data = new MyDataDataContext();

        public ActionResult Index()
        {
            var Hang = data.HangGPLXes.Select(p => p.MaHang).ToList();
            ViewBag.HangGPLX = new SelectList(Hang, "TenHang");
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = data.Users.SingleOrDefault(p => p.username == username && p.password == password);
            if (user != null)
            {
                Session["username"] = user.username;
                Session["password"] = user.password;
                Session["nameUser"] = user.nameUser;
                Session["role"] = user.idRole;
                if (user.idRole == 1)
                {
                    return RedirectToAction("DanhSachHoSo", "Admin/HoSo");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["error"] = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["username"] = null;
            Session["password"] = null;
            Session["role"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection formcollection, User user)
        {
            var nameUser = formcollection["nameUser"];
            var username = formcollection["username"];
            var password = formcollection["password"];

            if (string.IsNullOrEmpty(username))
            {
                TempData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(password))
            {
                TempData["Loi2"] = "Phải nhập mật khẩu";
            }
            else if (string.IsNullOrEmpty(nameUser))
            {
                TempData["Loi3"] = "Phải nhập tên người dùng";
            }
            else if (data.Users.Count(p => p.username == username) > 0)
            {
                TempData["Loi4"] = "Tên đăng nhập đã tồn tại";
            }
            else
            {
                user.nameUser = nameUser;
                user.username = username;
                user.password = password;
                user.idRole = 2;
                data.Users.InsertOnSubmit(user);
                data.SubmitChanges();
                return RedirectToAction("Login");
            }
            return this.Register();
        }

        [HttpPost]
        public ActionResult TraCuuHoSo(string MaGPLX, string HangGPLX)
        {
            var HoSo = (from hs in data.HoSoGPLXes
                        join ll in data.LyLiches on hs.SoCCCD equals ll.SoCCCD
                        join px in data.PhuongXas on ll.DiaChi equals px.MaPX
                        join qh in data.QuanHuyens on px.MaHuyen equals qh.MaHuyen
                        join t in data.TinhTPs on qh.MaTinh equals t.MaTinh
                        join dt in data.DanTocs on ll.MaDT equals dt.MaDT
                        join qt in data.QuocTiches on ll.MaQT equals qt.MaQT
                        join ttsh in data.TrungTamSatHaches on hs.MaTT equals ttsh.MaTT
                        where hs.MaGPLX == MaGPLX && hs.MaHang == HangGPLX
                        select new HoSo
                        {
                            HinhAnh = hs.HinhAnh,
                            MaGPLX = hs.MaGPLX,
                            HoTen = ll.HoLot + " " + ll.Ten,
                            NgaySinh = String.Format("{0:dd/MM/yyyy}", ll.NgaySinh),
                            GioiTinh = ll.GioiTinh,
                            SDT = ll.SDT,
                            DanToc = dt.TenDT,
                            DiaChi = px.TenPX + ", " + qh.TenHuyen + ", " + t.TenTinh,
                            QuocTich = qt.TenQT,
                            NgayCap = String.Format("{0:dd/MM/yyyy}", hs.NgayCapGPLX),
                            NgayHetHan = String.Format("{0:dd/MM/yyyy}", hs.NgayHetHanGPLX),
                            HangGPLX = hs.MaHang,
                            DiemLT = hs.DiemLT.ToString(),
                            DiemTH = hs.DiemTH.ToString(),
                            TTSH = ttsh.TenTT
                        }).First();
            return PartialView(HoSo);
        }
    }
}