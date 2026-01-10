(() => {
    const pageData = window.grPageData || { items: [], existingItems: [] };
    const itemsTableBody = document.getElementById('itemsTableBody');
    const noItemsMessage = document.getElementById('noItemsMessage');
    const addItemBtn = document.getElementById('addItemBtn');
    const purchaseOrderSelect = document.getElementById('purchaseOrderSelect');
    const vendorNameInput = document.querySelector('input[name="GR.VendorName"]');
    const formEl = document.querySelector('form');

    const itemsLookup = new Map();
    pageData.items.forEach(item => itemsLookup.set(item.id, item));

    let itemIndex = 0;

    function formatDate(value) {
        if (!value) return '';
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return '';
        return date.toISOString().split('T')[0];
    }

    function toggleNoItems() {
        if (!noItemsMessage || !itemsTableBody) return;
        noItemsMessage.style.display = itemsTableBody.children.length === 0 ? 'block' : 'none';
    }

    function buildItemOptions(selectedId) {
        const options = ['<option value="">-- Select --</option>'];
        pageData.items.forEach(item => {
            const selected = item.id === selectedId ? 'selected' : '';
            options.push(
                `<option value="${item.id}" data-uom="${item.unitOfMeasure || ''}" data-isbatch="${String(item.isBatch).toLowerCase()}" data-isexpiry="${String(item.isExpiry).toLowerCase()}" data-isserial="${String(item.isSerial).toLowerCase()}" ${selected}>${item.code} - ${item.name}</option>`
            );
        });
        return options.join('');
    }

    function updateItemFlagsForRow(row) {
        const select = row.querySelector('.item-select');
        const uomInput = row.querySelector('.item-uom');
        const batchInput = row.querySelector('.batch-field');
        const expiryInput = row.querySelector('.expiry-field');
        const serialInput = row.querySelector('.serial-field');
        const serialLabel = row.querySelector('.serial-label');

        const selectedOption = select?.options[select.selectedIndex];
        const isBatch = selectedOption?.dataset.isbatch === 'true';
        const isExpiry = selectedOption?.dataset.isexpiry === 'true';
        const isSerial = selectedOption?.dataset.isserial === 'true';
        const uom = selectedOption?.dataset.uom || '';

        row.querySelector('.item-isbatch').value = isBatch;
        row.querySelector('.item-isexpiry').value = isExpiry;
        row.querySelector('.item-isserial').value = isSerial;

        if (uomInput && uom) {
            uomInput.value = uom;
        }

        if (batchInput) {
            if (isBatch) {
                batchInput.removeAttribute('readonly');
                batchInput.classList.remove('bg-gray-50', 'border-gray-200');
                batchInput.classList.add('border-gray-300');
                batchInput.required = true;
                batchInput.placeholder = 'Batch';
            } else {
                batchInput.setAttribute('readonly', 'readonly');
                batchInput.classList.add('bg-gray-50', 'border-gray-200');
                batchInput.classList.remove('border-gray-300');
                batchInput.required = false;
                batchInput.placeholder = 'N/A';
                batchInput.value = '';
            }
        }

        if (expiryInput) {
            if (isExpiry) {
                expiryInput.removeAttribute('readonly');
                expiryInput.classList.remove('bg-gray-50', 'border-gray-200');
                expiryInput.classList.add('border-gray-300');
                expiryInput.required = true;
            } else {
                expiryInput.setAttribute('readonly', 'readonly');
                expiryInput.classList.add('bg-gray-50', 'border-gray-200');
                expiryInput.classList.remove('border-gray-300');
                expiryInput.required = false;
                expiryInput.value = '';
            }
        }

        if (serialInput) {
            if (isSerial) {
                serialInput.removeAttribute('readonly');
                serialInput.classList.remove('bg-gray-50', 'border-gray-200');
                serialInput.classList.add('border-gray-300');
                serialInput.required = true;
                serialInput.placeholder = 'Enter Serial';
                if (serialLabel) {
                    serialLabel.textContent = '*';
                    serialLabel.classList.remove('text-gray-400');
                    serialLabel.classList.add('text-red-500');
                }
            } else {
                serialInput.setAttribute('readonly', 'readonly');
                serialInput.classList.add('bg-gray-50', 'border-gray-200');
                serialInput.classList.remove('border-gray-300');
                serialInput.required = false;
                serialInput.placeholder = 'N/A';
                serialInput.value = '';
                if (serialLabel) {
                    serialLabel.textContent = 'N/A';
                    serialLabel.classList.add('text-gray-400');
                    serialLabel.classList.remove('text-red-500');
                }
            }
        }
    }

    function createRow(itemData = {}) {
        if (!itemsTableBody) return;

        const row = document.createElement('tr');
        row.dataset.itemIndex = itemIndex;

        const selectedItemId = itemData.itemId || null;
        const orderedQuantity = itemData.orderedQuantity ?? itemData.quantity ?? 0;
        const receivedQuantity = itemData.receivedQuantity ?? itemData.quantity ?? 0;
        const unitOfMeasure = itemData.unitOfMeasure || '';
        const batchNumber = itemData.batchNumber || '';
        const expiryDate = formatDate(itemData.expiryDate);
        const serialNumber = itemData.serialNumber || '';
        const grItemId = itemData.id || 0;
        const goodsReceiptId = itemData.goodsReceiptId || 0;

        row.innerHTML = `
            <input type="hidden" name="GR.Items[${itemIndex}].Id" value="${grItemId}" />
            <input type="hidden" name="GR.Items[${itemIndex}].GoodsReceiptId" value="${goodsReceiptId}" />
            <input type="hidden" class="item-isbatch" value="false" />
            <input type="hidden" class="item-isexpiry" value="false" />
            <input type="hidden" class="item-isserial" value="false" />
            <td class="px-3 py-2">
                <select name="GR.Items[${itemIndex}].ItemId" class="item-select w-full px-2 py-1 border border-gray-300 rounded text-sm" required>
                    ${buildItemOptions(selectedItemId)}
                </select>
            </td>
            <td class="px-3 py-2">
                <input type="number" name="GR.Items[${itemIndex}].OrderedQuantity" value="${orderedQuantity}" step="0.001" class="w-24 px-2 py-1 border border-gray-300 rounded text-sm" required />
            </td>
            <td class="px-3 py-2">
                <input type="number" name="GR.Items[${itemIndex}].ReceivedQuantity" value="${receivedQuantity}" step="0.001" class="w-24 px-2 py-1 border border-gray-300 rounded text-sm" required />
            </td>
            <td class="px-3 py-2">
                <input type="text" name="GR.Items[${itemIndex}].UnitOfMeasure" value="${unitOfMeasure}" class="item-uom w-20 px-2 py-1 border border-gray-300 rounded text-sm" required />
            </td>
            <td class="px-3 py-2">
                <input type="text" name="GR.Items[${itemIndex}].BatchNumber" value="${batchNumber}" class="batch-field w-24 px-2 py-1 border border-gray-200 rounded text-sm bg-gray-50" placeholder="N/A" readonly />
            </td>
            <td class="px-3 py-2">
                <input type="date" name="GR.Items[${itemIndex}].ExpiryDate" value="${expiryDate}" class="expiry-field w-32 px-2 py-1 border border-gray-200 rounded text-sm bg-gray-50" readonly />
            </td>
            <td class="px-3 py-2">
                <div class="flex items-center gap-1">
                    <input type="text" name="GR.Items[${itemIndex}].SerialNumber" value="${serialNumber}" class="serial-field w-32 px-2 py-1 border border-gray-200 rounded text-sm bg-gray-50" placeholder="N/A" readonly />
                    <span class="serial-label text-gray-400 text-sm">N/A</span>
                </div>
            </td>
            <td class="px-3 py-2 text-center">
                <button type="button" class="delete-row text-red-600 hover:text-red-800">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        `;

        itemsTableBody.appendChild(row);

        const selectEl = row.querySelector('.item-select');
        selectEl?.addEventListener('change', () => updateItemFlagsForRow(row));

        const deleteBtn = row.querySelector('.delete-row');
        deleteBtn?.addEventListener('click', () => {
            row.remove();
            toggleNoItems();
        });

        updateItemFlagsForRow(row);

        itemIndex++;
        toggleNoItems();
    }

    function resetItemsTable() {
        if (!itemsTableBody) return;
        itemsTableBody.innerHTML = '';
        itemIndex = 0;
        toggleNoItems();
    }

    function seedExistingItems() {
        if (!Array.isArray(pageData.existingItems) || pageData.existingItems.length === 0) {
            toggleNoItems();
            return;
        }
        pageData.existingItems.forEach(item => createRow(item));
    }

    async function handlePurchaseOrderChange() {
        const poId = purchaseOrderSelect?.value;
        if (!poId) return;

        try {
            const response = await fetch(`?handler=PODetails&poId=${poId}`);
            const data = await response.json();

            if (data.success === false) {
                alert(data.message);
                return;
            }

            if (vendorNameInput) {
                vendorNameInput.value = data.vendorName || '';
            }

            resetItemsTable();
            if (Array.isArray(data.items) && data.items.length > 0) {
                data.items.forEach(poItem => createRow({
                    itemId: poItem.itemId,
                    orderedQuantity: poItem.quantity,
                    receivedQuantity: poItem.quantity,
                    unitOfMeasure: poItem.unitOfMeasure
                }));
            }
        } catch (error) {
            console.error('Error loading PO details:', error);
            alert('Failed to load PO details');
        }
    }

    function validateForm(e) {
        const rows = itemsTableBody?.querySelectorAll('tr') || [];
        if (rows.length === 0) {
            e.preventDefault();
            alert('กรุณาเพิ่มรายการสินค้าอย่างน้อย 1 รายการ');
            return;
        }

        for (const row of rows) {
            const isBatch = row.querySelector('.item-isbatch')?.value === 'true';
            const isExpiry = row.querySelector('.item-isexpiry')?.value === 'true';
            const isSerial = row.querySelector('.item-isserial')?.value === 'true';

            if (isBatch) {
                const batchInput = row.querySelector('.batch-field');
                if (!batchInput?.value.trim()) {
                    e.preventDefault();
                    batchInput?.focus();
                    alert('กรุณากรอก Batch Number สำหรับสินค้าที่บังคับ Batch tracking');
                    return;
                }
            }

            if (isExpiry) {
                const expiryInput = row.querySelector('.expiry-field');
                if (!expiryInput?.value.trim()) {
                    e.preventDefault();
                    expiryInput?.focus();
                    alert('กรุณากรอก Expiry Date สำหรับสินค้าที่บังคับ Expiry tracking');
                    return;
                }
            }

            if (isSerial) {
                const serialInput = row.querySelector('.serial-field');
                if (!serialInput?.value.trim()) {
                    e.preventDefault();
                    serialInput?.focus();
                    alert('กรุณากรอก Serial Number สำหรับสินค้าที่บังคับ Serial tracking');
                    return;
                }
            }
        }
    }

    function wireEvents() {
        addItemBtn?.addEventListener('click', () => createRow());
        purchaseOrderSelect?.addEventListener('change', handlePurchaseOrderChange);
        formEl?.addEventListener('submit', validateForm);
    }

    wireEvents();
    seedExistingItems();
    toggleNoItems();
})();


