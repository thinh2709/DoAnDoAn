$(document).ready(function () {
    // Khởi tạo datatable cho bảng lịch thăm bệnh
    var tableLichKham = $('#tableLichThamBenh').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        },
        "order": [[3, "asc"]], // Sắp xếp theo thời gian thăm
        "columnDefs": [
            { "width": "15%", "targets": 6 } // Cột trạng thái
        ]
    });

    // Xử lý chức năng tạo lịch thăm bệnh
    $('#formTaoLichTham').on('submit', function (e) {
        e.preventDefault();
        
        var formData = $(this).serialize();
        
        $.ajax({
            url: '/LichThamBenh/Tao',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    toastr.success('Đăng ký lịch thăm bệnh thành công!');
                    $('#modalTaoLichTham').modal('hide');
                    location.reload();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi đăng ký lịch thăm bệnh!');
            }
        });
    });

    // Xử lý tìm kiếm bệnh nhân khi đăng ký thăm
    $('#timKiemBenhNhan').on('keyup', function () {
        var keyword = $(this).val().toLowerCase();
        
        $.ajax({
            url: '/BenhNhan/TimKiem',
            type: 'GET',
            data: { keyword: keyword },
            success: function (data) {
                var html = '';
                $.each(data, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + item.MaBenhNhan + '</td>';
                    html += '<td>' + item.HoTen + '</td>';
                    html += '<td>' + item.Khoa + '</td>';
                    html += '<td><button class="btn btn-sm btn-primary btn-chon-benhnhan" data-id="' + item.MaBenhNhan + '" data-ten="' + item.HoTen + '">Chọn</button></td>';
                    html += '</tr>';
                });
                
                $('#danhSachBenhNhan tbody').html(html);
            }
        });
    });

    // Xử lý chọn bệnh nhân từ danh sách tìm kiếm
    $(document).on('click', '.btn-chon-benhnhan', function () {
        var maBenhNhan = $(this).data('id');
        var tenBenhNhan = $(this).data('ten');
        
        $('#MaBenhNhan').val(maBenhNhan);
        $('#tenBenhNhanHienThi').text(tenBenhNhan);
        $('#modalTimBenhNhan').modal('hide');
    });

    // Xử lý chức năng duyệt lịch thăm
    $('.btn-duyet-lichtham').on('click', function () {
        var maLich = $(this).data('id');
        
        $.ajax({
            url: '/LichThamBenh/Duyet/' + maLich,
            type: 'POST',
            success: function (result) {
                if (result.success) {
                    toastr.success('Duyệt lịch thăm bệnh thành công!');
                    location.reload();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi duyệt lịch thăm bệnh!');
            }
        });
    });

    // Xử lý chức năng huỷ lịch thăm
    $('.btn-huy-lichtham').on('click', function () {
        var maLich = $(this).data('id');
        
        if (confirm('Bạn có chắc chắn muốn huỷ lịch thăm bệnh này?')) {
            $.ajax({
                url: '/LichThamBenh/Huy/' + maLich,
                type: 'POST',
                success: function (result) {
                    if (result.success) {
                        toastr.success('Huỷ lịch thăm bệnh thành công!');
                        location.reload();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function () {
                    toastr.error('Có lỗi xảy ra khi huỷ lịch thăm bệnh!');
                }
            });
        }
    });

    // Khởi tạo datetime picker cho thời gian thăm
    $('#ThoiGianTham').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        minDate: moment().add(1, 'hours'),
        stepping: 15
    });
});