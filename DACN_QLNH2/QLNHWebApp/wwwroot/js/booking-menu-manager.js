// Booking Menu Manager - No Reload Version
let bookingId;
let currentCategory = 'all';

function initBookingMenuManager(id) {
    bookingId = id;
    setupEventListeners();
}

function setupEventListeners() {
    // Tìm kiếm món ăn
    const searchInput = document.getElementById('searchMenuItem');
    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const searchTerm = e.target.value.toLowerCase();
            filterMenuItems(searchTerm, currentCategory);
        });
    }

    // Filter theo category
    document.querySelectorAll('.category-filter .badge').forEach(badge => {
        badge.addEventListener('click', function () {
            document.querySelectorAll('.category-filter .badge').forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            currentCategory = this.dataset.category;
            const searchTerm = searchInput ? searchInput.value.toLowerCase() : '';
            filterMenuItems(searchTerm, currentCategory);
        });
    });
}

function filterMenuItems(searchTerm, category) {
    const items = document.querySelectorAll('.menu-item-search');

    items.forEach(item => {
        const name = item.dataset.name;
        const itemCategory = item.dataset.category;

        const matchSearch = !searchTerm || name.includes(searchTerm);
        const matchCategory = category === 'all' || itemCategory === category;

        item.style.display = (matchSearch && matchCategory) ? '' : 'none';
    });
}

function selectMenuItem(menuItemId, menuItemName, price) {
    // Đóng modal
    const modal = bootstrap.Modal.getInstance(document.getElementById('addMenuItemModal'));
    if (modal) modal.hide();

    Swal.fire({
        title: `<i class="fas fa-utensils"></i> ${menuItemName}`,
        html: `
            <div class="text-start">
                <p class="mb-3"><strong>Giá:</strong> <span class="text-success">${price.toLocaleString()}₫</span></p>
                <label class="form-label"><strong>Số lượng:</strong></label>
            </div>
        `,
        input: 'number',
        inputValue: 1,
        inputAttributes: {
            min: 1,
            max: 100,
            step: 1,
            class: 'form-control form-control-lg text-center'
        },
        showCancelButton: true,
        confirmButtonText: '<i class="fas fa-check me-1"></i>Thêm vào đặt bàn',
        cancelButtonText: '<i class="fas fa-times me-1"></i>Hủy',
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        inputValidator: (value) => {
            if (!value || value < 1) return 'Số lượng phải lớn hơn 0!'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            addMenuItem(menuItemId, parseInt(result.value), menuItemName, price);
        }
    });
}

function addMenuItem(menuItemId, quantity, menuItemName, price) {
    showLoading();

    $.ajax({
        url: '/AdminBooking/AddItemToBooking',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            bookingId: bookingId,
            menuItemId: menuItemId,
            quantity: quantity
        }),
        success: function (response) {
            if (response.success) {
                showToast(`Đã thêm ${quantity}x ${menuItemName}`, 'success');

                // Cập nhật hoặc thêm món vào danh sách
                if (response.isNew) {
                    addNewItemToList(response.item);
                } else {
                    updateExistingItem(response.item);
                }

                updateGrandTotal();
            } else {
                showError(response.message);
            }
        },
        error: function () {
            showError('Không thể kết nối đến server');
        }
    });
}

function addNewItemToList(item) {
    const orderItemsList = document.getElementById('orderItemsList');

    // Nếu đang hiển thị message rỗng, xóa đi
    const emptyMessage = orderItemsList.querySelector('.text-center.py-5');
    if (emptyMessage) {
        orderItemsList.innerHTML = `
            <div class="table-responsive">
                <table class="table table-hover">
                    <thead class="table-light">
                        <tr>
                            <th style="width: 50%">Món ăn</th>
                            <th style="width: 20%" class="text-center">Số lượng</th>
                            <th style="width: 20%" class="text-end">Thành tiền</th>
                            <th style="width: 10%" class="text-center">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                    <tfoot>
                        <tr class="table-active">
                            <td colspan="2" class="text-end"><strong>Tổng cộng:</strong></td>
                            <td class="text-end">
                                <strong class="text-success fs-5" id="grandTotal">0₫</strong>
                            </td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        `;
    }

    const tbody = orderItemsList.querySelector('tbody');
    const newRow = document.createElement('tr');
    newRow.className = 'order-item-row';
    newRow.dataset.itemId = item.id;
    newRow.style.opacity = '0';
    newRow.innerHTML = `
        <td>
            <strong>${item.menuItemName}</strong><br>
            <small class="text-muted">${item.price.toLocaleString()}₫ / món</small>
        </td>
        <td class="text-center">
            <div class="quantity-control d-inline-flex">
                <button type="button" class="btn btn-sm btn-outline-secondary quantity-btn" 
                        onclick="updateQuantity(${item.id}, ${item.quantity - 1})">
                    <i class="fas fa-minus"></i>
                </button>
                <span class="quantity-display fw-bold" id="qty-${item.id}">${item.quantity}</span>
                <button type="button" class="btn btn-sm btn-outline-secondary quantity-btn" 
                        onclick="updateQuantity(${item.id}, ${item.quantity + 1})">
                    <i class="fas fa-plus"></i>
                </button>
            </div>
        </td>
        <td class="text-end">
            <strong class="item-total" id="total-${item.id}">${item.total.toLocaleString()}₫</strong>
        </td>
        <td class="text-center">
            <button type="button" class="btn btn-sm btn-danger" onclick="removeItem(${item.id})">
                <i class="fas fa-trash"></i>
            </button>
        </td>
    `;

    tbody.appendChild(newRow);

    // Fade in animation
    setTimeout(() => {
        newRow.style.transition = 'all 0.3s ease';
        newRow.style.opacity = '1';
    }, 10);
}

function updateExistingItem(item) {
    document.getElementById(`qty-${item.id}`).textContent = item.quantity;
    document.getElementById(`total-${item.id}`).textContent = item.total.toLocaleString() + '₫';
}

function updateQuantity(orderItemId, newQuantity) {
    if (newQuantity < 1) {
        removeItem(orderItemId);
        return;
    }

    const row = document.querySelector(`tr[data-item-id="${orderItemId}"]`);
    const priceText = row.querySelector('small.text-muted').textContent;
    const price = parseInt(priceText.replace(/[^\d]/g, ''));

    $.ajax({
        url: '/AdminBooking/UpdateItemQuantityInBooking',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            bookingId: bookingId,
            orderItemId: orderItemId,
            quantity: newQuantity
        }),
        success: function (response) {
            if (response.success) {
                // Cập nhật UI
                document.getElementById(`qty-${orderItemId}`).textContent = newQuantity;
                const newTotal = price * newQuantity;
                document.getElementById(`total-${orderItemId}`).textContent = newTotal.toLocaleString() + '₫';
                updateGrandTotal();
                showToast('Đã cập nhật', 'success');
            } else {
                showError(response.message);
            }
        },
        error: function () {
            showError('Không thể cập nhật');
        }
    });
}

function removeItem(orderItemId) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: 'Bạn có chắc muốn xóa món này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '<i class="fas fa-trash me-1"></i>Xóa',
        cancelButtonText: '<i class="fas fa-times me-1"></i>Hủy',
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/AdminBooking/RemoveItemFromBooking',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    bookingId: bookingId,
                    orderItemId: orderItemId
                }),
                success: function (response) {
                    if (response.success) {
                        // Xóa dòng với animation
                        const row = document.querySelector(`tr[data-item-id="${orderItemId}"]`);
                        row.style.transition = 'all 0.3s ease';
                        row.style.opacity = '0';
                        row.style.transform = 'translateX(-20px)';

                        setTimeout(() => {
                            row.remove();
                            updateGrandTotal();
                            checkEmptyList();
                        }, 300);

                        showToast('Đã xóa món', 'success');
                    } else {
                        showError(response.message);
                    }
                },
                error: function () {
                    showError('Không thể xóa món');
                }
            });
        }
    });
}

function updateGrandTotal() {
    let total = 0;
    document.querySelectorAll('.order-item-row').forEach(row => {
        const totalText = row.querySelector('.item-total').textContent;
        const itemTotal = parseInt(totalText.replace(/[^\d]/g, ''));
        total += itemTotal;
    });

    const grandTotalEl = document.getElementById('grandTotal');
    if (grandTotalEl) {
        grandTotalEl.textContent = total.toLocaleString() + '₫';
    }
}

function checkEmptyList() {
    const tbody = document.querySelector('#orderItemsList tbody');
    if (!tbody || tbody.children.length === 0) {
        document.getElementById('orderItemsList').innerHTML = `
            <div class="text-center py-5 text-muted">
                <i class="fas fa-utensils fa-3x mb-3"></i>
                <p class="mb-0">Chưa có món ăn nào. Nhấn nút "Thêm món" để bắt đầu.</p>
            </div>
        `;
    }
}

function showLoading() {
    Swal.fire({
        title: 'Đang xử lý...',
        html: '<i class="fas fa-spinner fa-spin fa-3x text-primary"></i>',
        showConfirmButton: false,
        allowOutsideClick: false
    });
}

function showToast(message, type = 'success') {
    const toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true
    });

    toast.fire({
        icon: type,
        title: message
    });

    Swal.close();
}

function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: message
    });
}
