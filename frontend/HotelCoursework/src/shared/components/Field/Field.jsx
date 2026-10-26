import { useState } from 'react'
import styles from './Field.module.css'
import { memo } from 'react'
const Field = (props)=>{
    const {
        className='',
        id,
        label,
        isPassword=false,
        type="text",
        onChange,
        error='',
        value,
        disabled=false,

    } = props
    const [isHidden,setIsHidden] = useState(true)

    const toggleShowPassword = ()=>{
        setIsHidden(!isHidden)
    }
    return(
        <div className={`${styles['field']} ${className}`}>
            <label htmlFor={id}>{label}</label>
            {!isPassword ?
            <>
        <input 
             type={type} 
             className={`${styles.input} ${error ? styles['input-error'] :''}`} 
             onChange={onChange}
             disabled={disabled}
             id={id}
             value={value}
             />
             {error && <span className={styles['error']}>{error}</span>}
            </>
             : 
            (isHidden ?
                <div className={styles['input-password-wrapper']}>
                    <input 
                    type='password' 
                    className={`
                        ${styles['input-password']} 
                        ${styles['hidden']}
                        ${error ? styles['input-error'] :''}
                        `} 
                    id={id}
                    disabled={disabled}
                    onChange={onChange} 
                    value={value}
                    />
                    <button 
                    className={styles['show-button']} 
                    onClick={toggleShowPassword}
                    type='button'></button>
                    {error && <span className={styles['error']}>{error}</span>}
                </div>
                
             :
             <div className={styles['input-password-wrapper']}>
                <input 
                type='text' 
                className={`${styles['input-password'] } ${error ? styles['input-error'] :''}`} 
                id={id}
                disabled={disabled}
                onChange={onChange} 
                value={value}
                />
                <button 
                className={`${styles['show-button']} ${styles['show']}`} 
                onClick={toggleShowPassword}
                type='button'
                ></button>
                {error && <span className={styles['error']}>{error}</span>}
             </div>)
}
        </div>
    )
}

export default memo(Field)