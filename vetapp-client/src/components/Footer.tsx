const Footer = () => {
  const currentYear: number = new Date().getFullYear()
  return (
    <>
      <footer className="bg-slate-800 text-white">
        <div className="container mx-auto py-8 text-center">
          &copy; {currentYear} VetApp. All Rights reserved.
        </div>
      </footer>
    </>
  )
}

export default Footer;
