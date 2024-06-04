$(document).ready(function () {
    $(document).on('click', document, function (e) {
        // 監聽畫面上的點擊事件
        const targetCustomSelect = e.target.closest('.custom-select')

        if (targetCustomSelect === null) {
            // 如果沒有點到選項相關元素，收起所有選項
            $('.custom-options-child-layer').addClass('d-none')
            $('.custom-options').addClass('d-none')

            $('.custom-select').removeClass('expanded')
        }
    })

    // 監聽選取框點擊事件
    $(document).on('click', '.custom-select-value', function () {
        const $targetCustomSelect = $(this).closest('.custom-select')
        const selectType = $targetCustomSelect.data('select-type')
        const isExpanded = $targetCustomSelect.hasClass('expanded')

        $('.custom-select').removeClass('expanded')
        $('.custom-options-child-layer').addClass('d-none')
        $('.custom-options').addClass('d-none')

        if (!isExpanded) {
            $targetCustomSelect.addClass('expanded')
            if (selectType === "multi-layer") {
                // 如果點到有多層的選取框，顯示該選取框第一層選項
                $targetCustomSelect.find('.custom-options-layer1').removeClass('d-none')
            } else {
                // 如果點到沒有多層的選取框，顯示選項
                $targetCustomSelect.find('.custom-options').removeClass('d-none')
                // 如果點到沒有多層的選取框，隱藏多層的選項並移除被選取樣式
                $('.custom-option-muti-layer[data-layer-index="1"]').removeClass('custom-selected-option')
            }
        }
    })

    // 單選選取框選項 點擊事件
    $(document).on('click', '.custom-option-single', function () {
        const $currentCustomSelect = $(this).closest('.custom-select')
        // 新增、移除被選取樣式
        $(this).toggleClass('custom-selected-option')
        const $totalSelectedOption = $currentCustomSelect.find(`.custom-selected-option`)
        $totalSelectedOption.removeClass('custom-selected-option')
        $(this).toggleClass('custom-selected-option')
        //更新選取框文字
        $currentCustomSelect.find('.custom-select-value-text').text($(this).data('value'))
        //隱藏選項
        $currentCustomSelect.find(`.custom-options`).addClass('d-none')
        $currentCustomSelect.removeClass('expanded')

        if (afterSingleOptionClick != undefined) {
            afterSingleOptionClick($(this))
        }
    })

    // 非多層選取框選項 點擊事件
    $(document).on('click', '.custom-option', function () {
        // 新增、移除被選取樣式
        $(this).toggleClass('custom-selected-option')
        // 更新被選取總數
        const $currentCustomSelect = $(this).closest('.custom-select')
        const totalSelectedOption = $currentCustomSelect.find(`.custom-selected-option`)
        const totalNumString = totalSelectedOption.length === 0 ? $currentCustomSelect.data('placeholder') : `已選${totalSelectedOption.length}`
        $currentCustomSelect.find('.custom-select-value-text').text(`${totalNumString}`)
    })

    // 多層選取框選項 點擊事件
    $(document).on('click', '.custom-option-muti-layer', function () {
        const layerIndex = $(this).data('layer-index')
        if (layerIndex === 1) {
            // 如果點擊的是第一層：
            // 切換是否被選取樣式
            $('.custom-option-muti-layer[data-layer-index="1"]').removeClass('custom-selected-option')
            $(this).toggleClass('custom-selected-option')
            // 顯示第二層
            $(this).closest('.custom-select').find('.custom-options-layer2').removeClass('d-none')
            // 顯示第二層對應的選項
            const optionId = $(this).data('option-id')
            const $childOption = $(`.custom-option-muti-layer[data-parent-option="${optionId}"]`)
            $(`.custom-option-muti-layer[data-layer-index="2"]`).addClass('d-none')
            $childOption.removeClass('d-none')
        } else if (layerIndex === 2) {
            // 如果點擊的是第二層：
            // 切換是否被選取樣式
            $(this).toggleClass('custom-selected-option')
            // 更新第一層選項 子層被選取選項數量
            const parentOptionId = $(this).data('parent-option')
            const $parentOption = $(`.custom-option-muti-layer[data-option-id="${parentOptionId}"]`)
            const selectedOptionWithSameId = $(`.custom-selected-option[data-parent-option="${parentOptionId}"]`)
            const parentOptionText = $parentOption.data('option-text')
            const numString = selectedOptionWithSameId.length === 0 ? "" : `(${selectedOptionWithSameId.length})`
            $parentOption.text(`${parentOptionText}${numString}`)

            // 更新選取框的數量
            const $currentCustomSelect = $(this).closest('.custom-select')
            const totalSelectedOption = $currentCustomSelect.find(`.custom-selected-option[data-layer-index="2"]`)
            const totalNumString = totalSelectedOption.length === 0 ? $currentCustomSelect.data('placeholder') : `已選${totalSelectedOption.length}`
            $currentCustomSelect.find('.custom-select-value-text').text(`${totalNumString}`)
        }
    })

    // 監聽房租上限下限數值
    $(document).on('change', '.rent-limit', function () {
        changeRentText()
    })

    // 替換房租文字
    function changeRentText() {
        const upperLimit = $('#rent-upper-limit').val()
        const lowerLimit = $('#rent-lower-limit').val()
        const $currentCustomSelect = $('#rent-custom-select')
        let rentLimitString = $currentCustomSelect.data('placeholder')
        if (upperLimit !== "" && lowerLimit !== "") {
            rentLimitString = `${lowerLimit}~${upperLimit}元`
        } else if (upperLimit === "" && lowerLimit !== "") {
            rentLimitString = `${lowerLimit}元以上`
        } else if (lowerLimit === "" && upperLimit !== "") {
            rentLimitString = `${upperLimit}元以下`
        }

        $currentCustomSelect.find('.custom-select-value-text').text(rentLimitString)
    }
    changeRentText()
})