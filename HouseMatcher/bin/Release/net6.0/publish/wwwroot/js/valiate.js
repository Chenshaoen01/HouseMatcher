
function validateBeforeSubmit(targetValidDataList, submitValueData) {
    let hasInValidColumn = false
    targetValidDataList.forEach(validateData => {
        const taregtDataValue = submitValueData !== undefined ? submitValueData[validateData.columnKey] : $(`#${validateData.columnId}`).val()
        const isTargetColumnValid = valiateTargetColumn(validateData, taregtDataValue)
        hasInValidColumn = hasInValidColumn || !isTargetColumnValid
    })

    return !hasInValidColumn
}

// 檢驗特定欄位特定規則，回傳：是否符合規則
function valiateTargetColumn(validateData, targetValue) {
    let isValid = true
    for (let index = 0; isValid && index < validateData.rule.length; index++) {
        isValid = validateWithRule(targetValue, validateData, index)
    }

    return isValid
}

// 檢驗特定欄位特定規則，回傳：是否符合規則
function validateWithRule(targetValue, validateData, ruleIndex) {
    const ruleName = validateData.rule[ruleIndex]

    let isValidValue = true
    if (ruleName === 'required') {
        isValidValue = targetValue !== null && targetValue !== "" && targetValue !== undefined
    } else if (ruleName === 'email') {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        isValidValue = emailRegex.test(targetValue)
    } else if (ruleName === 'positive-number') {
        isValidValue = targetValue > 0
    }

    let inValidHintText = ""
    if (!isValidValue) {
        if (ruleName === 'required') {
            inValidHintText = `${validateData.columnName}為必填項目`
        } else if (ruleName === 'email') {
            inValidHintText = `${validateData.columnName}格式不符`
        } else if (ruleName === 'positive-number') {
            inValidHintText = `${validateData.columnName}須為正數`
        }
    }
    $(`#${validateData.validateTextId}`).text(inValidHintText)

    return isValidValue
}