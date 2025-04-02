$(document).ready(function () {
    // Khởi tạo datatable cho bảng chi phí điều trị
    var tableChiPhi = $('#tableChiPhiDieuTri').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        },
        "order": [[3, "desc"]], // Sắp xếp theo ngày lập
        "columnDefs": [
            { 
                "targets": 2, 
                "render": function (data, type, row) {
                    return formatCurrency(data);
                }
            },
            {
                "targets": 4,
                "orderable": false
            }
        ]
    });

    // Hàm định dạng tiền tệ
    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }

    // Xử lý chức năng tạo hóa đơn chi phí
    $('#formTaoChiPhi').on('submit', function (e) {
        e.preventDefault();
        
        var formData = $(this).serialize();
        
        $.ajax({
            url: '/ChiPhi/Tao',
            type: 'POST',
            data: formData,
            success: function (result) {
                if (result.success) {
                    toastr.success('Tạo hóa đơn chi phí thành công!');
                    $('#modalTaoChiPhi').modal('hide');
                    location.reload();
                } else {
                    toastr.error(result.message);
                }
            },
            error: function () {
                toastr.error('Có lỗi xảy ra khi tạo hóa đơn chi phí!');
            }
        });
    });

    // Xử lý chức năng thanh toán chi phí
    $('.btn-thanhtoan').on('click', function () {
        var maChiPhi = $(this).data('id');
        
        if (confirm('Xác nhận thanh toán hóa đơn này?')) {
            $.ajax({
                url: '/ChiPhi/ThanhToan/' + maChiPhi,
                type: 'POST',
                success: function (result) {
                    if (result.success) {
                        toastr.success('Thanh toán hóa đơn thành công!');
                        location.reload();
                    } else {
                        toastr.error(result.message);
                    }
                },
                error: function () {
                    toastr.error('Có lỗi xảy ra khi thanh toán hóa đơn!');
                }
            });
        }
    });

    // Xử lý chức năng xem chi tiết chi phí
    $('.btn-chitiet').on('click', function () {
        var maBenhNhan = $(this).data('benhnhan');
        
        $.ajax({
            url: '/ChiPhi/ChiTiet/' + maBenhNhan,
            type: 'GET',
            success: function (data) {
                var html = '';
                var tongCong = 0;
                
                $.each(data, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.TenDieuTri + '</td>';
                    html += '<td>' + formatCurrency(item.ChiPhi) + '</td>';
                    html += '<td>' + moment(item.NgayThucHien).format('DD/MM/YYYY') + '</td>';
                    html += '</tr>';
                    
                    tongCong += item.ChiPhi;
                });
                
                $('#chiTietBody').html(html);
                $('#tongCongChiPhi').text(formatCurrency(tongCong));
                
                $('#modalChiTietChiPhi').modal('show');
            },
            error: function () {
                toastr.error('Không thể lấy chi tiết chi phí!');
            }
        });
    });

    // Xử lý tính toán chi phí khi chọn dịch vụ điều trị
    $('#MaDieuTri').on('change', function () {
        var maDieuTri = $(this).val();
        
        if (maDieuTri) {
            $.ajax({
                url: '/DieuTri/LayThongTin/' + maDieuTri,
                type: 'GET',
                success: function (data) {
                    $('#ChiPhi').val(data.ChiPhi);
                    $('#ChiPhiHienThi').text(formatCurrency(data.ChiPhi));
                }
            });
        } else {
            $('#ChiPhi').val('');
            $('#ChiPhiHienThi').text('');
        }
    });

    // Xử lý in hóa đơn
    $('.btn-in-hoadon').on('click', function () {
        var maChiPhi = $(this).data('id');
        window.open('/ChiPhi/InHoaDon/' + maChiPhi, '_blank');
    });

    // Tính tổng chi phí khi thêm nhiều dịch vụ
    $('.them-dichvu').on('click', function () {
        var dichVuRow = $('.row-dichvu:first').clone();
        dichVuRow.find('select, input').val('');
        dichVuRow.find('.chiPhiDichVu').text('');
        
        $('#containerDichVu').append(dichVuRow);
        tinhTongChiPhi();
    });

    // Xóa dịch vụ
    $(document).on('click', '.xoa-dichvu', function () {
        if ($('.row-dichvu').length > 1) {
            $(this).closest('.row-dichvu').remove();
            tinhTongChiPhi();
        }
    });

    // Hàm tính tổng chi phí
    function tinhTongChiPhi() {
        var tong = 0;
        $('.chiPhiDichVu').each(function () {
            var chiPhi = parseFloat($(this).data('chiphi')) || 0;
            tong += chiPhi;
        });
        
        $('#tongChiPhi').text(formatCurrency(tong));
    }
});