
function validateBeforeSubmit(targetValidDataList) {
    let hasInValidColumn = false
    targetValidDataList.forEach(validateData => {
        const isTargetColumnValid = valiateTargetColumn(validateData)
        hasInValidColumn = hasInValidColumn || !isTargetColumnValid
    })

    return !hasInValidColumn
}

// 檢驗特定欄位特定規則，回傳：是否符合規則
function valiateTargetColumn(validateData) {
    let isValid = true
    for (let index = 0; isValid && index < validateData.rule.length; index++) {
        isValid = validateWithRule(validateData, index)
    }

    return isValid
}

// 檢驗特定欄位特定規則，回傳：是否符合規則
function validateWithRule(validateData, ruleIndex) {
    const targetValue = $(`#${validateData.columnId}`).val()
    const ruleName = validateData.rule[ruleIndex]

    let isValidValue = true
    if (ruleName === 'required') {
        isValidValue = targetValue !== null && targetValue !== "" && targetValue !== undefined
    } else if (ruleName === 'email') {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        isValidValue = emailRegex.test(targetValue)
    }

    let inValidHintText = ""
    if (!isValidValue) {
        if (ruleName === 'required') {
            inValidHintText = `${validateData.columnName}為必填項目`
        } else if (ruleName === 'email') {
            inValidHintText = `${validateData.columnName}格式不符`
        }
    }
    $(`#${validateData.validateTextId}`).text(inValidHintText)

    return isValidValue
}