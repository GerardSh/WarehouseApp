function changeCurrency() {
    const symbol = document.getElementById("currency").value;

    document.querySelectorAll('.unit-price').forEach(td => {
        const val = parseFloat(td.getAttribute('data-value'));
        td.textContent = symbol + ' ' + val.toFixed(2);
    });

    document.querySelectorAll('.total-price').forEach(td => {
        const val = parseFloat(td.getAttribute('data-value'));
        td.textContent = symbol + ' ' + val.toFixed(2);
    });

    const totalSpan = document.getElementById('summaryTotal');
    const totalValue = parseFloat(totalSpan.getAttribute('data-value'));
    totalSpan.textContent = symbol + ' ' + totalValue.toFixed(2);
}

window.addEventListener('DOMContentLoaded', changeCurrency);