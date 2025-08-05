document.addEventListener('DOMContentLoaded', () => {
    const invoiceHelper = document.getElementById('invoice-helper');
    const productHelperWrapper = document.getElementById('product-helper-wrapper');
    const warehouseId = window.warehouseId;

    if (!invoiceHelper || !productHelperWrapper || !warehouseId) {
        console.warn('Missing DOM elements or warehouseId is not defined.');
        return;
    }

    fetch(`/api/exportdata/warehouse/${warehouseId}/invoices`)
        .then(res => res.json())
        .then(invoices => {
            invoiceHelper.innerHTML = '';

            if (invoices.length === 0) {
                const noInvoicesMsg = document.createElement('p');
                noInvoicesMsg.textContent = 'No invoices with available products found.';
                noInvoicesMsg.style.color = 'gray';
                invoiceHelper.parentElement.insertBefore(noInvoicesMsg, invoiceHelper);
                return;
            }

            invoices.forEach(inv => {
                const p = document.createElement('p');
                p.textContent = inv;

                p.style.display = 'inline-block';
                p.style.fontSize = '1.1rem';
                p.style.marginRight = '10px';
                p.style.cursor = 'pointer';
                p.style.color = '#007bff';
                p.style.textDecoration = 'underline';

                p.addEventListener('mouseover', () => {
                    p.style.color = '#0056b3';
                });

                p.addEventListener('mouseout', () => {
                    p.style.color = '#007bff';
                });

                p.addEventListener('click', () => loadProductsForInvoice(inv));

                invoiceHelper.appendChild(p);
            });
        })
        .catch(err => {
            console.error('Error fetching invoices:', err);
        });

    function loadProductsForInvoice(invoiceNumber) {
        fetch(`/api/exportdata/warehouse/${warehouseId}/invoices/${invoiceNumber}/products`)
            .then(res => res.json())
            .then(products => {
                productHelperWrapper.innerHTML = '';

                // Add invoice number heading
                const invoiceHeading = document.createElement('h5');
                invoiceHeading.textContent = `${invoiceNumber}`;
                invoiceHeading.style.marginBottom = '12px';
                invoiceHeading.style.color = '#ffffff';
                invoiceHeading.style.fontSize = '1.1rem';
                invoiceHelper.style.fontWeight = 'normal';
                productHelperWrapper.appendChild(invoiceHeading);

                // Create table and header
                const table = document.createElement('table');
                table.style.width = '100%';
                table.style.borderCollapse = 'collapse';
                table.style.color = '#cbd6e8';
                table.style.fontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";

                const thead = document.createElement('thead');
                const headerRow = document.createElement('tr');

                ['Product', 'Category', 'Available'].forEach(headerText => {
                    const th = document.createElement('th');
                    th.textContent = headerText;
                    th.style.borderBottom = '1.5px solid #3a5a81';
                    th.style.padding = '8px';
                    th.style.textAlign = 'left';
                    th.style.fontWeight = '600';
                    headerRow.appendChild(th);
                });

                thead.appendChild(headerRow);
                table.appendChild(thead);

                const tbody = document.createElement('tbody');

                products.forEach(p => {
                    const tr = document.createElement('tr');
                    tr.style.backgroundColor = '#1e2a38';

                    const productTd = document.createElement('td');
                    productTd.textContent = p.name;
                    productTd.style.padding = '8px';
                    productTd.style.fontWeight = '600';
                    productTd.style.backgroundColor = '#1e2a38'; 
                    tr.appendChild(productTd);

                    const categoryTd = document.createElement('td');
                    categoryTd.textContent = p.category;
                    categoryTd.style.padding = '8px';
                    categoryTd.style.fontStyle = 'italic';
                    categoryTd.style.backgroundColor = '#1e2a38'; 
                    tr.appendChild(categoryTd);

                    const quantityTd = document.createElement('td');
                    quantityTd.textContent = p.availableQuantity;
                    quantityTd.style.padding = '8px';
                    quantityTd.style.textAlign = 'center';
                    quantityTd.style.backgroundColor = '#1e2a38';
                    tr.appendChild(quantityTd);

                    tbody.appendChild(tr);
                });

                table.appendChild(tbody);
                productHelperWrapper.appendChild(table);

                productHelperWrapper.style.cssText = `
                    display: block;
                    background-color: #1e2a38;
                    border: 1.5px solid #3a5a81;
                    padding: 16px 20px;
                    border-radius: 8px;
                    max-width: 420px;
                    margin-top: 15px;
                    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.6);
                    overflow-x: auto;
                `;
            });
    }
});
