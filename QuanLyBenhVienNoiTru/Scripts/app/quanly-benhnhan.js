$(document).ready(function () {
    // Khởi tạo datatable cho bảng bệnh nhân
    var tableBenhNhan = $('#tableBenhNhan').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        }
    });

    // Xử lý chức năng thêm bệnh nhân
    $('#formThemBenhNhan').on('submit', function (e) {
        e.preventDefault();
        
        var formData = $(this).serialize();
        
        $.ajax({
            url: '/BenhNhan/Them',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    toastr.success('Thêm bệnh nhân thành công!');
                    $('#modalThemBenhNhan').modal('hide');
                    location.reload();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi thêm bệnh nhân!');
            }
        });
    });

    // Xử lý chức năng sửa bệnh nhân
    $('.btn-sua-benhnhan').on('click', function () {
        var maBenhNhan = $(this).data('id');
        
        $.ajax({
            url: '/BenhNhan/LayThongTin/' + maBenhNhan,
            type: 'GET',
            success: function (data) {
                $('#editMaBenhNhan').val(data.MaBenhNhan);
                $('#editHoTen').val(data.HoTen);
                $('#editNgaySinh').val(data.NgaySinh);
                $('#editGioiTinh').val(data.GioiTinh);
                $('#editSoDienThoai').val(data.SoDienThoai);
                $('#editDiaChi').val(data.DiaChi);
                $('#editBaoHiemYTe').prop('checked', data.BaoHiemYTe);
                $('#editNgayNhapVien').val(data.NgayNhapVien);
                $('#editNgayXuatVien').val(data.NgayXuatVien);
                $('#editMaKhoa').val(data.MaKhoa);
                
                $('#modalSuaBenhNhan').modal('show');
            },
            error: function () {
                toastr.error('Không thể lấy thông tin bệnh nhân!');
            }
        });
    });

    // Xử lý lưu thông tin sửa bệnh nhân
    $('#formSuaBenhNhan').on('submit', function (e) {
        e.preventDefault();
        
        var formData = $(this).serialize();
        
        $.ajax({
            url: '/BenhNhan/CapNhat',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    toastr.success('Cập nhật thông tin bệnh nhân thành công!');
                    $('#modalSuaBenhNhan').modal('hide');
                    location.reload();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi cập nhật thông tin!');
            }
        });
    });

    // Xử lý chức năng xoá bệnh nhân
    $('.btn-xoa-benhnhan').on('click', function () {
        var maBenhNhan = $(this).data('id');
        
        if (confirm('Bạn có chắc chắn muốn xoá bệnh nhân này?')) {
            $.ajax({
                url: '/BenhNhan/Xoa/' + maBenhNhan,
                type: 'POST',
                success: function (result) {
                    if (result.success) {
                        toastr.success('Xoá bệnh nhân thành công!');
                        location.reload();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function () {
                    toastr.error('Có lỗi xảy ra khi xoá bệnh nhân!');
                }
            });
        }
    });

    // Xử lý chức năng xuất viện
    $('.btn-xuatvien').on('click', function () {
        var maBenhNhan = $(this).data('id');
        
        if (confirm('Xác nhận xuất viện cho bệnh nhân này?')) {
            $.ajax({
                url: '/BenhNhan/XuatVien/' + maBenhNhan,
                type: 'POST',
                success: function (result) {
                    if (result.success) {
                        toastr.success('Cập nhật xuất viện thành công!');
                        location.reload();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function () {
                    toastr.error('Có lỗi xảy ra khi cập nhật xuất viện!');
                }
            });
        }
    });
});