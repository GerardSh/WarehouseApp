// Initialize productIndex with the count of products from the server model on page load
document.addEventListener('DOMContentLoaded', function () {
    let productIndex = window.productIndex || 0;

    checkIfOnlyOneRow();

    $(document).ready(function () {
        // ✅ Ensure there's always at least one row on load
        const $container = $('#products-container');
        if ($container.children('tr.product-group').length === 0) {
            addProduct();
        }

        checkIfOnlyOneRow();
    });

    window.addProduct = function () {
        const $container = $('#products-container');

        // Use the current count of product rows as the index for the new product
        const newIndex = $container.children('tr.product-group').length;

        const html = `
                <tr class="product-group">
                    <td>
                        <input type="text"
                               name="Products[${newIndex}].ProductName"
                               class="form-control bg-dark text-white"
                               placeholder="Product name"
                               data-val="true"
                               data-val-required="Product name is required."
                               data-val-minlength="Product name must be at least 3 characters."
                               data-val-minlength-min="3"
                               data-val-maxlength="Product name cannot be longer than 255 characters."
                               data-val-maxlength-max="255" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].ProductName"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <input type="text"
                               name="Products[${newIndex}].ProductDescription"
                               class="form-control bg-dark text-white"
                               placeholder="Product description"
                               data-val="true"
                               data-val-minlength="Product description must be at least 5 characters."
                               data-val-minlength-min="5"
                               data-val-maxlength="Product description cannot be longer than 1000 characters."
                               data-val-maxlength-max="1000" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].ProductDescription"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <input type="number"
                               name="Products[${newIndex}].Quantity"
                               class="form-control bg-dark text-white"
                               placeholder="Quantity"
                               data-val="true"
                               data-val-required="Quantity is required."
                               data-val-range="Quantity must be between 1 and 2147483647."
                               data-val-range-min="1"
                               data-val-range-max="2147483647" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].Quantity"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <input type="number"
                               step="0.01"
                               name="Products[${newIndex}].UnitPrice"
                               class="form-control bg-dark text-white"
                               placeholder="Unit price"
                               data-val="true"
                               data-val-range="Unit Price must be between 0 and 9007199254740991."
                               data-val-range-min="0"
                               data-val-range-max="9007199254740991" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].UnitPrice"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <input type="text"
                               name="Products[${newIndex}].CategoryName"
                               class="form-control bg-dark text-white"
                               placeholder="Category name"
                               data-val="true"
                               data-val-required="Category name is required."
                               data-val-minlength="Category name must be at least 3 characters."
                               data-val-minlength-min="3"
                               data-val-maxlength="Category name cannot be longer than 255 characters."
                               data-val-maxlength-max="255" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].CategoryName"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <input type="text"
                               name="Products[${newIndex}].CategoryDescription"
                               class="form-control bg-dark text-white"
                               placeholder="Category description"
                               data-val="true"
                               data-val-minlength="Category description must be at least 5 characters."
                               data-val-minlength-min="5"
                               data-val-maxlength="Category description cannot be longer than 1000 characters."
                               data-val-maxlength-max="1000" />
                        <span class="text-danger field-validation-valid"
                              data-valmsg-for="Products[${newIndex}].CategoryDescription"
                              data-valmsg-replace="true"></span>
                    </td>
                    <td>
                        <button type="button" class="remove-btn" onclick="removeProduct(this)">Remove</button>
                    </td>
                </tr>`;

        $container.append(html);

        // Reparse validation after adding new inputs
        const $form = $("#importInvoiceForm");
        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);

        // Update productIndex to be ready for the next addition
        productIndex = $container.children('tr.product-group').length;

        checkIfOnlyOneRow();  // <-- call here AFTER adding a product if none exist
    };

    window.removeProduct = function (button) {
        const $rows = $('#products-container tr.product-group');

        // ✅ Prevent deleting the last row
        if ($rows.length <= 1) return;

        // Remove the product row
        $(button).closest('tr').remove();

        // Reindex remaining products so their names and validation attributes stay continuous
        reindexProducts();

        checkIfOnlyOneRow();
    };

    function reindexProducts() {
        const $rows = $('#products-container tr.product-group');

        $rows.each(function (index) {
            // Update inputs and validation spans in the current row
            $(this).find('input, span[data-valmsg-for]').each(function () {
                if (this.tagName.toLowerCase() === 'input') {
                    const name = $(this).attr('name');
                    if (!name) return;

                    const newName = name.replace(/Products\[\d+\]/, `Products[${index}]`);
                    $(this).attr('name', newName);

                    // Optional: update id attribute if present
                    const id = $(this).attr('id');
                    if (id) {
                        const newId = id.replace(/Products_\d+__/, `Products_${index}__`);
                        $(this).attr('id', newId);
                    }
                }

                if ($(this).attr('data-valmsg-for')) {
                    const valmsgFor = $(this).attr('data-valmsg-for');
                    const newValmsgFor = valmsgFor.replace(/Products\[\d+\]/, `Products[${index}]`);
                    $(this).attr('data-valmsg-for', newValmsgFor);
                }
            });
        });

        checkIfOnlyOneRow();

        // Reset productIndex to current number of rows
        productIndex = $rows.length;

        // Reparse validation after renaming inputs
        const $form = $("#importInvoiceForm");
        $form.removeData("validator");
        $form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);
    }

    function checkIfOnlyOneRow() {
        const rows = document.querySelectorAll('#products-container tr.product-group');

        if (rows.length === 1) {
            const deleteBtn = rows[0].querySelector('.remove-btn');
            if (deleteBtn) {
                deleteBtn.disabled = true;
                deleteBtn.classList.add('disabled-btn');  // add disabled style class
                deleteBtn.innerHTML = '🔒';
            }
        } else {
            rows.forEach(row => {
                const deleteBtn = row.querySelector('.remove-btn');
                if (deleteBtn) {
                    deleteBtn.disabled = false;
                    deleteBtn.classList.remove('disabled-btn');  // remove disabled style
                    deleteBtn.innerHTML = 'Remove';
                }
            });
        }
    }
});
